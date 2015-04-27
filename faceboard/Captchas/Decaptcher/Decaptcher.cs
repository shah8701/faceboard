using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace BotGuruz.Utils
{
    class Decaptcher
    {
        /* 
         * Global initialization of decaptcher module, called one time in primary thread.
         * Return Value:
         * If the function succeeds, the return value is 0.
         * If the function fails, the return value is -1;
         * */
        [DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int DecaptcherInit();

        /*
         * Initialization decaptcher, for thread. Called one time in each thread.
         * Return Value:
         * If the function succeeds, the return value id >= 0.
         * If the function fails, the return value is less than 0;
         * -201 - threads limit
         * -202 - can't allocate memory
         * */
        [DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int CCprotoInit();

        /*
         * Login in decaptcher system.
         * Parameters:
         * id         - context(returned by function CCprotoInit()).
         * hostname   - ip or the host name of decaptcher.
         * port       - your port.
         * login      - your login.
         * login_size - login length in bytes.
         * pwd        - your password.
         * pwd_size   - password length in bytes.
         * Return Value:
         * If the function succeeds, the return value is 0.
         * If the function fails, the return value is less than 0.
         * -201 - incorrect size login or password.
         * -202 - incorrect id.
         * 	*/
        [DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int CCprotoLogin(int id, string hostname, int port, string login, int login_size, string pwd, int pwd_size);

        /*
         * Get balance
         * Parameters:
         * id - context(returned by function CCprotoInit())
         * balance - this is the pointer to the balance value
         * Return Value:
         * If the function succeeds, the return 0.
         * If the function fails, the return value is less than 0;
         * -201 - incorrect id.
         * -202 - pointer balance zero
         * */
        [DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
    //    unsafe public static extern int CCprotoBalance(int id, float* balance);


        /*
         * Send captcha.
         * Parameters:
         * id        - context(returned by function CCprotoInit())
         * pict      - buffer with picture
         * pict_size - size of picture buffer
         * text      - returned text, the buffer must be preallocated size 100 byte or more
         * Return Value:
         * If the function succeeds, the return value is 0 - ccERR_OK.
         * If the function fails, the return value is <0
         * ccERR_GENERAL		= -1   - server-side error, better to call ccproto_login() again.
         * ccERR_STATUS        = -2   - local error. either ccproto_init() or ccproto_login() has not been successfully called prior to ccproto_picture(), need ccproto_init() and ccproto_login() to be called.
         * ccERR_NET_ERROR     = -3   - network troubles, better to call ccproto_login() again.
         * ccERR_TEXT_SIZE     = -4   - size of the text returned is too big.
         * ccERR_OVERLOAD      = -5   - temporarily server-side error, server's overloaded, wait a little before sending a new picture.
         * ccERR_BALANCE       = -6   - not enough funds to process a picture, balance is depleted.
         * ccERR_TIMEOUT       = -7   - picture has been timed out on server (payment not taken).
         * ccERR_UNKNOWN       = -200 - unknown error, better to call ccproto_login() again.
         *						 -201 - incorrect id.
         *						 -202 - incorrect pict_size
         * */
      ///  [DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
       // unsafe public static extern int CCprotoPicture(int id, byte* pict, int pict_size, byte* text);


        /*
         * Send captcha.
         * Parameters:
         * id          - context(returned by function CCprotoInit())
         * pict        - buffer with picture
         * pict_size   - size of picture buffer
         * 
         * p_pict_to   - IN/OUT timeout specifier to be used, on return - really used specifier
         * ptoDEFAULT		= 0, use default server's timeout
         * ptoLONG			= 1, wait infinitely (be cautious with this) 
         * pto30SEC		    = 2, 30 seconds
         * pto60SEC   		= 3, 60 seconds
         * pto90SEC  		= 4, 90 seconds
         * 
         * p_pict_type - IN/OUT	type specifier to be used, on return - really used specifier
         * ptUNSPECIFIED	= 0, picture type is unspecified, no special tratment is needed
         * 
         * text        - returned text, the buffer must be preallocated size 100 byte or more
         * size_buf    - IN/OUT	size of text's buffer on IN, size of the text received - on OUT
         * major_id    - OUT major part of the picture ID
         * minor_id 	- OUT minor part of the picture ID
         * Return Value:
         * If the function succeeds, the return value is 0 - ccERR_OK.
         * If the function fails, the return value is <0
         * ccERR_GENERAL		= -1   - server-side error, better to call ccproto_login() again.
         * ccERR_STATUS        = -2   - local error. either ccproto_init() or ccproto_login() has not been successfully called prior to ccproto_picture(), need ccproto_init() and ccproto_login() to be called.
         * ccERR_NET_ERROR     = -3   - network troubles, better to call ccproto_login() again.
         * ccERR_TEXT_SIZE     = -4   - size of the text returned is too big.
         * ccERR_OVERLOAD      = -5   - temporarily server-side error, server's overloaded, wait a little before sending a new picture.
         * ccERR_BALANCE       = -6   - not enough funds to process a picture, balance is depleted.
         * ccERR_TIMEOUT       = -7   - picture has been timed out on server (payment not taken).
         * ccERR_UNKNOWN       = -200 - unknown error, better to call ccproto_login() again.
         *						 -201 - incorrect id.
         *						 -202 - incorrect pict_size or size_buf
         * */
       //[DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
       // unsafe public static extern int CCprotoPicture2(int id, byte* pict, int pict_size, int* p_pict_to, int* p_pict_type, byte* text, int size_buf, int* major_id, int* minor_id);

        /*
         * Report last picture in context as bad.
         * Parameters:
         * id - context(returned by function CCprotoInit())
         * Return Value:
         * If the function succeeds, the return value is 0.
         * If the function fails, the return value is <0.
         * -201 - incorrect id.
         * */
      //  [DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int CCprotoPictureBad(int id);

        /*
         * Report picture as bad.
         * Parameters:
         * id       - context(returned by function CCprotoInit()).
         * major_id - major part of the picture ID.
         * minor_id - minor part of the picture ID.
         * Return Value:
         * If the function succeeds, the return value is 0.
         * If the function fails, the return value is <0
         * -201 - incorrect id
         * */
        [DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int CCprotoPictureBad2(int id, int major_id, int minor_id);

        /*
         * Terminate the session and close context
         * Parameters:
         * id - context(returned by function CCprotoInit())
         * Return Value:
         * If the function succeeds, the return value is 0.
         * If the function fails, the return value is <0.
         * -1 - incorrect id.
         * */
        [DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int CCprotoClose(int id);

        /*
         * Destroy the context before the thread exit
         * Parameters:
         * id - context(returned by function CCprotoInit())
         * Return Value:
         * If the function succeeds, the return value is 0.
         * If the function fails, the return value is <0.
         * -1 - incorrect id.
         * */
        [DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int CCprotoDestroy(int id);

        /*
         * Destroy decaptcher module, call before programm ends.
         * Return Value:
         * If the function succeeds, the return value is 0.
         * If the function fails, the return value is <0.
         * */
        [DllImport("decaptcher.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int DecaptcherDestroy(int id);



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
       // [STAThread]
        //unsafe public void Decaptcher1234()
        //{

        //    int id;
        //    int ret;
        //    int pic_size;

        //    int[] p_pict_to;
        //    int[] p_pict_type;
        //    int size_buf;
        //    int[] major_id;
        //    int[] minor_id;

        //    float[] balance;

        //    if (DecaptcherInit() == -1) return;

        //    id = CCprotoInit();

        //    if (id == -1) return;

        //    ret = CCprotoLogin(id, "72.233.64.162", 4707, "kingtickets", 11, "1tt3y4a0qd3m7d9sxqw2i", 21);

        //    if (ret < 0) return;

        //    balance = new float[1];
        //    fixed (float* balance1 = &balance[0])

        //        ret = CCprotoBalance(id, balance1);

        //    if (ret < 0) return;

        //    FileStream fs = new FileStream("pic.jpg", FileMode.Open);// replace pic.jpg to your image path

        //    byte[] buffer = new byte[fs.Length];
        //    pic_size = (int)fs.Length;
        //    fs.Read(buffer, 0, buffer.Length);
        //    fs.Close();



        //    byte[] captcha1 = new byte[256];
        //    p_pict_to = new int[1];
        //    p_pict_type = new int[1];

        //    major_id = new int[1];
        //    minor_id = new int[1];

        //    p_pict_to[0] = 0;
        //    p_pict_type[0] = 0;
        //    size_buf = 255;


        //    fixed (int* p_pict_to1 = &p_pict_to[0])
        //    fixed (int* p_pict_type1 = &p_pict_type[0])
        //    fixed (int* major_id1 = &major_id[0])
        //    fixed (int* minor_id1 = &minor_id[0])
        //    fixed (byte* captcha = &captcha1[0])
        //    fixed (byte* bufPass = &buffer[0])
        //    {
        //        //ret = CCprotoPicture(id,bufPass,pic_size, captcha);
        //        ret = CCprotoPicture2(id, bufPass, pic_size, p_pict_to1, p_pict_type1, captcha, size_buf, major_id1, minor_id1);
        //    }


        //    //if captcha recognized incorrectly then call CCprotoPictureBad2(id,major_id[0],minor_id[0]); 


        //    string s = "";

        //    s = new string(Encoding.ASCII.GetChars(captcha1));


        //    CCprotoClose(id);




        //}

        string DecaptchaHost = string.Empty;
        int DecaptchaPort = 0;
        string DecaptchaLogin = string.Empty;
        string DecaptchaPassword = string.Empty;

        //unsafe public string decaptcha(byte[] img, string DecaptchaHost, int DecaptchaPort, string DecaptchaLogin, string DecaptchaPassword)
        //{
        //    unsafe
        //    {
        //        int id;
        //        int ret;
        //        int pic_size;

        //        int[] p_pict_to;
        //        int[] p_pict_type;
        //        int size_buf;
        //        int[] major_id;
        //        int[] minor_id;

        //        float[] balance;

        //        if (Decaptcher.DecaptcherInit() == -1) return "";

        //        id = Decaptcher.CCprotoInit();

        //        if (id == -1) return "";

        //        ret = Decaptcher.CCprotoLogin(id, DecaptchaHost, DecaptchaPort, DecaptchaLogin, DecaptchaLogin.Length, DecaptchaPassword, DecaptchaPassword.Length);

        //        if (ret < 0)
        //        {

        //            if (ret == -1)
        //            {
        //               // MessageBox.Show("Decaptcher.com_Message : server-side error, better to call Decaptcher_login() again.");
        //                //  Log("Decaptcher.com_Message : server-side error, better to call Decaptcher_login() again."); 
        //            }
        //            else if (ret == -2)
        //            {
        //               // MessageBox.Show("Decaptcher.com_Message : local error. either ccproto_init() or Decaptcher_login() has not been successfully called prior to ccproto_picture(), need ccproto_init() and Decaptcher_login() to be called.");
        //                // Log("Decaptcher.com_Message : local error");
        //            }
        //            else if (ret == -3)
        //            {
        //               // MessageBox.Show("Decaptcher.com_Message : network troubles, better to call Decaptcher_login() again.");
        //                //Log("Decaptcher.com_Message : network troubles"); 
        //            }
        //            else if (ret == -4)
        //            {
        //               // MessageBox.Show("Decaptcher.com_Message : size of the text returned is too big.");
        //                // Log("Decaptcher.com_Message : size of the text returned is too big"); 
        //            }
        //            else if (ret == -5)
        //            {
        //              //  MessageBox.Show("Decaptcher.com_Message : temporarily server-side error, server's overloaded, wait a little before sending a new picture.");
        //            }
        //            else if (ret == -6)
        //            {
        //               // MessageBox.Show("Decaptcher.com_Message : not enough funds to process a picture, balance is depleted.");
        //                // Log("not enough funds to process a picture, balance is depleted");
        //            }
        //            else if (ret == -7)
        //            {
        //               // MessageBox.Show("Decaptcher.com_Message : picture has been timed out on server (payment not taken).");
        //                //  Log("Decaptcher.com_Message : picture has been timed out on server");
        //            }
        //            else if (ret == -200)
        //            {
        //               // MessageBox.Show("Decaptcher.com_Message : unknown error, better to call ccproto_login() again.");
        //                //  Log("Decaptcher.com_Message : unknown error, better to call ccproto_login() again.");
        //            }
        //            else if (ret == -201)
        //            {
        //               // MessageBox.Show("Decaptcher.com_Message : incorrect id.");
        //                // Log("Decaptcher.com_Message : incorrect id.");
        //            }
        //            else if (ret == -202)
        //            {
        //               // MessageBox.Show("Decaptcher.com_Message : incorrect pict_size");
        //                // Log("Decaptcher.com_Message : incorrect pict_size");
        //            }

        //            return "";
        //        }

        //        balance = new float[1];
        //        fixed (float* balance1 = &balance[0])

        //            ret = Decaptcher.CCprotoBalance(id, balance1);

        //        if (ret < 0) return "";

        //      //  FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Autoblogposter" + "\\" + img, FileMode.Open);
        //        //Bitmap IMGurl;
        //        //byte[] buffer = new byte[fs.Length];
        //        //pic_size = (int)fs.Length;
        //        //fs.Read(buffer, 0, buffer.Length);
        //        //fs.Close();

        //        byte[] buffer = img;
        //        pic_size = buffer.Length;
        //        byte[] captcha1 = new byte[256];
        //        p_pict_to = new int[1];
        //        p_pict_type = new int[1];

        //        major_id = new int[1];
        //        minor_id = new int[1];

        //        p_pict_to[0] = 0;
        //        p_pict_type[0] = 0;
        //        size_buf = 255;

        //        fixed (int* p_pict_to1 = &p_pict_to[0])
        //        fixed (int* p_pict_type1 = &p_pict_type[0])
        //        fixed (int* major_id1 = &major_id[0])
        //        fixed (int* minor_id1 = &minor_id[0])
        //        fixed (byte* captcha = &captcha1[0])
        //        fixed (byte* bufPass = &buffer[0])
        //        {
        //            //ret = CCprotoPicture(id,bufPass,pic_size, captcha);
        //            ret = Decaptcher.CCprotoPicture2(id, bufPass, pic_size, p_pict_to1, p_pict_type1, captcha, size_buf, major_id1, minor_id1);
        //            if (ret == -5)
        //            {
        //                //Logger.LogText("Decapther Server is overloaded Waiting for 5 secs and re-trying...", null);

        //                // decaptcha(string img);

        //            }
        //        }

        //        string s = "";
        //        s = new string(Encoding.ASCII.GetChars(captcha1));
        //        string result = s.Replace("\0", "");
        //        //if (result.ToString() == "")
        //        //{
        //        //    decaptcha();
        //        //}

        //        Decaptcher.CCprotoClose(id);
        //        // this.Invoke(objDecaptchaDelegate, s, true);
        //        // Logger.LogText("Captcha Text Fetched Successfully", null);
        //        return result;
        //    }
        //}
    }
}
