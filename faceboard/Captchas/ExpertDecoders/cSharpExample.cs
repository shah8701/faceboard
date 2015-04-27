
using System;
using System.Text;
using System.IO;
using System.Net;

namespace CaptchaClient
{
	
	public static class SampleUse
	{
	
		public static void SampleSolve()
		{
			string ExampleFile="test.jpeg";
			CaptchaSolver solver=new CaptchaSolver("http://www.fasttypers.org/imagepost.ashx",
			                                     "KLJFSDFJLLG9KJT5IOEIWE839823K");
			
			if (solver.SolveCaptcha(ExampleFile))
			{
				string CaptchaText=solver.LastResponseText;
				//if you think that captchaText is not correct
				//Claim your refund
				//like solver.RefundLastTask(); or solver.refund(solver.LastTaskId);
			}else
			{
				//do actions based solver.LastPostState
				
			}
		
		}
	}
	
	public class CaptchaSolver
	{
		#region Attributes
		private string _PostUrl;
		private string _AccessKey;
		private string _VendorKey;
		private string _LastResponseText;
		private string _LastTaskId;
		private ResponseState _LastPostState;
		#endregion
		
		#region AttributeMehodes
		public string LastTaskId {
			get { return _LastTaskId; }
		}
		public ResponseState LastPostState {
			get { return _LastPostState; }
		}
		public string LastResponseText {
			get { return _LastResponseText; }
		}
		public string AccessKey {
			get { return _AccessKey; }
		}
		public string VendorKey {
			get { return _VendorKey; }
		}
		public string PostUrl {
			get { return _PostUrl; }
		}
		#endregion
		
		public CaptchaSolver(string ImagePostUrl,string Key, string PartnerVendorKey)
		{
			this._PostUrl=ImagePostUrl;
			this._AccessKey=Key;
			this._VendorKey=PartnerVendorKey;
		}
	    public CaptchaSolver(string ImagePostUrl,string Key)
		{
			this._PostUrl=ImagePostUrl;
			this._AccessKey=Key;
			this._VendorKey="";
		}
		#region PrivateMethods
		private  string EncodeUrl(string str)
		{
			if (str == null) return "";

			Encoding enc = Encoding.ASCII;
			StringBuilder result = new StringBuilder();

			foreach (char symbol in str)
			{
				byte[] bs = enc.GetBytes(new char[] { symbol });
				for (int i = 0; i < bs.Length; i++)
				{
					byte b = bs[i];
					if (b >= 48 && b < 58 || b >= 65 && b < 65 + 26 || b >= 97 && b < 97 + 26) // decode non numalphabet
					{
						result.Append(Encoding.ASCII.GetString(bs, i, 1));
					}
					else
					{
						result.Append('%' + String.Format("{0:X2}", (int)b));
					}
				}
			}

			return result.ToString();
		}
		private  void Post( params string[] ps)
		{
			try
			{
				this._LastResponseText="";
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.PostUrl);
				request.Proxy = WebRequest.DefaultWebProxy;
				string str = "";
				
				for (int i = 0; i + 1 < ps.Length; i += 2)
				{
					str += EncodeUrl(ps[i]) + "=" + EncodeUrl(ps[i + 1]) + "&";
				}
				if (str.EndsWith("&"))
				{
					str = str.Substring(0, str.Length - 1);
				}

				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				byte[] buffer = Encoding.ASCII.GetBytes(str);
				request.ContentLength = buffer.Length;
				Stream newStream = request.GetRequestStream();
				newStream.Write(buffer, 0, buffer.Length);

				WebResponse response = request.GetResponse();
				Stream sStream = response.GetResponseStream();
				StreamReader reader = new StreamReader(sStream);
				string ResponseSt = reader.ReadToEnd();
				this.DecodeResponse(ResponseSt);
				reader.Close();
				response.Close();
				newStream.Close();
				
			}
			catch
			{
				this._LastResponseText="";
			}
		}
		
		
		private void DecodeResponse(string ResponseSt)
		{
			this._LastResponseText="";
			char[] split=new char[]{' '};
			string[]splitMessage=ResponseSt.Split(split,StringSplitOptions.RemoveEmptyEntries);
			if (splitMessage.Length>1 && splitMessage[0]=="Error")
			{
				
				switch(splitMessage[1])
				{
					case "INCORRECT_ID":
						this._LastPostState= ResponseState.INCORRECT_ACCESS_KEY;
						break;
					case "NOT_ENOUGH_FUND":
						this._LastPostState = ResponseState.NOT_ENOUGH_FUND;
						break;
					case "TIMEOUT":
						this._LastPostState= ResponseState.TIMEOUT;
						break;
					case "INVALID_REQUEST":
						this._LastPostState=ResponseState.INVALID_REQUEST;
						break;
					case "UNKNOWN":
						this._LastPostState=ResponseState.UNKNOWN;
						break;
					default:
						this._LastPostState= ResponseState.OK;
						break;
				}
				
			}
			else 
			{
				this._LastPostState= ResponseState.OK;
				this._LastResponseText=ResponseSt;
			}
			
			
		}
		#endregion
		#region PublicMethods
		public  bool SolveCaptcha(string imageFileLocation)
		{
			this._LastTaskId=Guid.NewGuid().ToString();

			// read image data
			byte[] buffer = File.ReadAllBytes(imageFileLocation);

			// base64 encode it
			string img = Convert.ToBase64String(buffer);

			// submit captcha to server
			Post( new string[] {
			     	"action","upload",
			     	"vendorkey",this.VendorKey,
			     	"key", this.AccessKey,
			     	"file", img,
			     	"gen_task_id", this._LastTaskId});
			
			if (this.LastPostState== ResponseState.OK)
			{
				return true ;
			}
			else
			{
				return false;
			}
		}
		public bool RefundLastTask()
		{
			return Refund(this.LastTaskId);
		}
		public  bool Refund( string task_id)
		{
			
			Post( new string[] {
			     	"action","refund",
			     	"key", this.AccessKey,
			     	"gen_task_id", task_id});
			
			if (this.LastPostState== ResponseState.OK)
			{
				return true;
			}
			else
			{
				return false;
			}
			
		}
		public  int Balance()
		{
			try
			{
				
				Post( new string[] {
				     	"action","balance",
				     	"key", this.AccessKey
				     });
				if (this.LastPostState== ResponseState.OK)
				{
					return Convert.ToInt32(this.LastResponseText);
				}
				else
				{
					return 0;
				}
			}
			catch
			{
				throw;
			}
		}
		#endregion
	}
	public enum ResponseState
	{
		OK,
		TIMEOUT,
		INVALID_REQUEST,
		INCORRECT_ACCESS_KEY,
		NOT_ENOUGH_FUND,
		UNKNOWN,
	}
}
