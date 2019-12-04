using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Timers;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Resources;




namespace JRC_JHS182_AIS_D
{
    /// <summary>
    /// Interaction logic for JRC_JHS182_AIS.xaml
    /// </summary>
    public partial class JRC_JHS182_AIS : UserControl
    {
        //逻辑控制变量，用于控制不同界面显示
        public static int m_iFunOrder = 0;
        //静态显示内容结构
        public MY_STRUCT m_Struct_No1 = new MY_STRUCT();
        //新计时器，显示当前时间
        public DispatcherTimer m_Timer = new DispatcherTimer();
        //新计时器，控制开机
        public DispatcherTimer m_PowOn = new DispatcherTimer();
        //新计时器，实时计算
        public DispatcherTimer m_RealTimeCal = new DispatcherTimer();  
        //新计时器，快速编辑
        public DispatcherTimer m_QuickEdit = new DispatcherTimer();
        //快速编辑参数，-1代表减，1代表加
        int m_iQuickEdit = 1;
        //其他船舶总数，暂时设为20
        static int m_iTotal = 20;
        int m_iOwnpos = 1;
        //控制是否在主界面显示本船信息
        bool m_bOwnposdisp = false;
        //控制关机
        bool m_bPower = false;
        //控制显示时间
        bool m_bUTC = false;
        //控制按键音
        bool m_bSound = false;
        //记录其他船数据的数组，暂时设为总其他船舶数行，30列
        string[,] m_strData = new string[m_iTotal+10, 30];
        //记录本船数据的数组
        string[] m_strOwnshipData = new string[30];
        //其他船舶数值属性存储数组，用于实时计算
        double[] m_dLAT = new double[m_iTotal];
        double[] m_dLON = new double[m_iTotal];
        float[] m_fCOG = new float[m_iTotal];
        float[] m_fSOG = new float[m_iTotal];
        float[] m_fROT = new float[m_iTotal];
        int[] m_iHDG = new int[m_iTotal];
        double []m_dRNG = new double[m_iTotal];
        double[] m_dBRG = new double[m_iTotal];
        double[] m_dCPA = new double[m_iTotal];
        float[] m_fTCPA = new float[m_iTotal];
        //本船数值属性存储数组，用于实时计算
        double m_dOwnLAT = 0;
        double m_dOwnLON = 0;
        float m_fOwnCOG = 0;
          float m_fOwnSOG = 0;
          float m_fOwnROT = 0;
         int m_iOwnHDG = 0;
        //本船静态数据
         int m_iOwnNavStatus = 0;
         int m_iOwnCargoStatus = 0;
         float m_fOwnDraught = 0;
         int m_iPersons = 16;
         float m_iHeightKeel = 30;
        //报警参数，以距离为基准
         float m_fGuardRNG = 6;
         float m_fDispRNG = 1.5f;
         bool m_bAutoRNG = false;
         float m_fLostRNG = 20;
        //航路点参数
         double[] m_WpLAT = new double[10];
         double[] m_WpLON = new double[10];
        //消息记录参数及数据
         int m_iMessage = 2;
         string[,] m_strMessage = new string[10, 30];
         string[,] m_strMessageReceive = new string[10, 30];
      //判断旋钮被按下
         int m_iRotateDown = 0;
        //其他参数
         int m_iNumOfShip = 22;
         int m_iContrast = 7;
         int m_iMMSiSel = 0;   
        //构造函数 
        public JRC_JHS182_AIS()
        {
            InitializeComponent();
            //响应句柄设置为TRUE
            Btn_MENU.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Button_MouseLeftButtonDown_MENU), true);
            Btn_MENU.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp_MENU), true);
            Btn_CLR.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Button_MouseLeftButtonDown_CLR), true);
            Btn_CLR.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp_CLR), true);
            Btn_DSPL.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Button_MouseLeftButtonDown_DSPL), true);
            Btn_DSPL.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp_DSPL), true);
            Btn_PWR.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Button_MouseLeftButtonDown_PWR), true);
            Btn_PWR.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp_PWR), true);
            Btn_OFF.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Button_MouseLeftButtonDown_OFF), true);
            Btn_OFF.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp_OFF), true);
            Btn_Stk_up.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Button_MouseLeftButtonDown_Stk_up), true);
            Btn_Stk_up.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp_Stk_up), true);
            Btn_Stk_down.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Button_MouseLeftButtonDown_Stk_down), true);
            Btn_Stk_down.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp_Stk_down), true);
            Btn_Stk_left.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Button_MouseLeftButtonDown_Stk_left), true);
            Btn_Stk_left.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp_Stk_left), true);
            Btn_Stk_right.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Button_MouseLeftButtonDown_Stk_right), true);
            Btn_Stk_right.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp_Stk_right), true);
            Btn_Stk.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Button_MouseLeftButtonDown_Stk), true);
            Btn_Stk.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp_Stk), true);
            Btn_Rt_right.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(right_MouseLeftButtonDown), true);
            Btn_Rt_right.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(right_MouseLeftButtonUp), true);
            Btn_Rt_left.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(left_MouseLeftButtonDown), true);
            Btn_Rt_left.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(left_MouseLeftButtonUp), true);
            Btn_Enter.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Btn_MouseLeftButtonDown_Enter), true);
            Btn_Enter.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Btn_Enter_MouseLeftButtonUp), true);

            //初始化各种信息 ，关机时才会调用
            Initial_My_Struct();
            //初始化按钮状态  
            Draw_My_Button(1, 0);
            //计时器，显示当前时间功能延迟和委托
            m_Timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            m_Timer.Tick += new EventHandler(Time_Display);
            //计时器，实时计算功能延迟和委托
            m_RealTimeCal.Interval=new TimeSpan(0, 0, 0, 0, 3000);
            m_RealTimeCal.Tick += new EventHandler(RealTime_Cal);
            m_PowOn.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            m_PowOn.Tick += new EventHandler(Power_On);
            //计时器，参数快速编辑
            m_QuickEdit.Interval=new TimeSpan(0,0,0,0,150);
            m_QuickEdit.Tick+=new EventHandler(Quick_Edit);
            //初始化本船信息，从debug文件夹数据库，Ownship表中读取
            initOwnship();         
            //关闭显示当前时间定时器
            m_Timer.Stop();
            //关闭实时计算定时器
            m_RealTimeCal.Stop();
            //关闭快速编辑计时器
            m_QuickEdit.Stop();
            //按钮游标隐藏
            image_HandOFF.Visibility = Visibility.Collapsed;
            image_HandPow.Visibility = Visibility.Collapsed;
            //初始化其他船舶信息，从debug文件夹数据库，Target表中读取（+1是因为数据库ID从1开始）
            for (int i = 1; i < m_iTotal+1; i++)
            {
                getConn(i);
            }
            //初始化发件箱和收件箱
               for (int i = 0; i < 10; i++)
                   for (int j = 0; j < 10; j++)
                   {
                       m_strMessage[i, j] = "";
                       m_strMessageReceive[i, j] = "";
                   }
            //初始化收件箱两条信息
              m_strMessageReceive[0, 0] = "LONGXING";
             m_strMessageReceive[0, 1] = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") + " " + DateTime.Now.AddHours(1).AddMinutes(27).ToString("hh:mm");
             m_strMessageReceive[0, 2] = 415354785.ToString();
             m_strMessageReceive[0, 3] = "CATAGORY:ROUTINE";
             m_strMessageReceive[0, 4] = "REPLY:ON";
             m_strMessageReceive[0, 5] = "FUNCTION:TEXT";
             m_strMessageReceive[0, 6] = "CH:AUTO";
             m_strMessageReceive[0, 7] = "HELLO!";
             m_strMessageReceive[1, 0] = "FENGJING";
             m_strMessageReceive[1, 1] = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") + " " + DateTime.Now.AddHours(-3).AddMinutes(-36).ToString("hh:mm");
             m_strMessageReceive[1, 2] = 425256263.ToString();
             m_strMessageReceive[1, 3] = "CATAGORY:ROUTINE";
             m_strMessageReceive[1, 4] = "REPLY:ON";
             m_strMessageReceive[1, 5] = "FUNCTION:TEXT";
             m_strMessageReceive[1, 6] = "CH:AUTO";
             m_strMessageReceive[1, 7] = "HI GUYS,WHAT'S UP?";
            //初始化发件箱两条信息
             m_strMessage[0, 0] = "YUEJING";
             m_strMessage[0, 1] = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") + " " + DateTime.Now.AddHours(-2).AddMinutes(45).ToString("hh:mm");
             m_strMessage[0, 2] = 412132142.ToString();
             m_strMessage[0, 3] = "CATAGORY:ROUTINE";
             m_strMessage[0, 4] = "REPLY:ON";
             m_strMessage[0, 5] = "FUNCTION:TEXT";
             m_strMessage[0, 6] = "CH:AUTO";
             m_strMessage[0, 7] = "GOOD DAY!";
             m_strMessage[1, 0] = "CANGTIAN";
             m_strMessage[1, 1] = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") + " " + DateTime.Now.AddHours(-5).AddMinutes(-13).ToString("hh:mm");
             m_strMessage[1, 2] = 432141421.ToString();
             m_strMessage[1, 3] = "CATAGORY:ROUTINE";
             m_strMessage[1, 4] = "REPLY:ON";
             m_strMessage[1, 5] = "FUNCTION:TEXT";
             m_strMessage[1, 6] = "CH:AUTO";
             m_strMessage[1, 7] = "I'M FINE";


            
          

            
           
          

            
                       
        }
        //屏幕静态显示内容数据结构
        public class MY_STRUCT
        {
          
            public String Menu_Title_Left;
            public String Menu_Title_Right;
            public String Menu_Title_Mid;
          
        }
        //控制屏幕左上角标题显示内容逻辑变量
        int m_iTitle_Choose = 1;
        
        //List功能，逻辑控制变量
        int m_iBRG_Disp = 0;
        int m_iSORT_Disp = 0;
        int m_iNAME_Disp = 0;
        //标题内容显示函数，参数为m_iTitle_Choose
        private void TitleDisplay(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        textBlock_Title.Text = "SORT:NORTH/RANGE";
                        break; }
                case 2:
                    {
                        textBlock_Title.Text = "SORT:HEAD/RANGE";
                        break; }
                case 3:
                    {
                        textBlock_Title.Text = "SORT:NORTH/TCPA";
                        break; }
                case 4:
                    { textBlock_Title.Text = "SORT:HEAD/TCPA"; 
                        break; }
                case 5:
                    {
                        textBlock_Title.Text = "SORT:NORTH/GROUP"; 
                        break; }
                case 6:
                    {
                        textBlock_Title.Text = "SORT:HEAD/GROUP";
                        break;
                    }
                case 7:
                    {
                        textBlock_Title.Text = "OWN DETAIL";
                        break;
                    }
                case 8:
                    {
                        textBlock_Title.Text = "OWN SHIP'S DETAIL";
                        break;
                    }
                case 9:
                    {
                        textBlock_Title.Text = "SHIP'S DETAIL";
                        break;
                    }
                case 10:
                    {
                        textBlock_Title.Text = "MAIN MENU";
                        break;
                    }
                case 11:
                    {
                        textBlock_Title.Text = "GRAPHIC DISP.SET";
                        break;
                    }
                case 60:
                    {
                        textBlock_Title.Text = "VOYAGE DATA SET";
                        break;
                    }
                case 606:
                    {
                        textBlock_Title.Text = "WAYPOINTS";
                        break;
                    }
                case 61:
                    {
                        textBlock_Title.Text = "MESSAGE";
                        break;
                    }
                case 610:
                    {
                        textBlock_Title.Text = "EDIT AND TX";
                        break;
                    }
                case 611:
                    {
                        textBlock_Title.Text = "TX TRAY";
                        break;
                    }
                case 612:
                    {
                        textBlock_Title.Text = "RX TRAY";
                        break;
                    }
                case 613:
                    {
                        textBlock_Title.Text = "INTERROGATION";
                        break;
                    }
                case 614:
                    {
                        textBlock_Title.Text = "LONG RANGE";
                        break;
                    }
                case 62:
                    {
                        textBlock_Title.Text = "USER ALM SETTING";
                        break;
                    }
                case 63:
                    {
                        textBlock_Title.Text = "SET UP";
                        break;
                    }
                case 64:
                    {
                        textBlock_Title.Text = "MAINTENANCE";
                        break;
                    }
                case 640:
                    {
                        textBlock_Title.Text = "SELF DIAGNOSIS";
                        break;
                    }
                case 641:
                    {
                        textBlock_Title.Text = "TRX CONDITION";
                        break;
                    }
                case 642:
                    {
                        textBlock_Title.Text = "ALARM HISTORY";
                        break;
                    }
                case 643:
                    {
                        textBlock_Title.Text = "SENSOR STATUS";
                        break;
                    }
                case 644:
                    {
                        textBlock_Title.Text = "POWER ON/OFF LOG";
                        break;
                    }
                case 645:
                    {
                        textBlock_Title.Text = "SOFTWARE VERSION";
                        break;
                    }



            }
        }
        //底部内容显示函数
        private void BottomDisplay()
        {
            if ((stackPanel_1_Guard.Children[m_Count_Choose] as TextBlock).Text == "G")
            {
                textBlock_Bottom.Text = " GUARD           MSG";
            }
            else if ((stackPanel_1_Guard.Children[m_Count_Choose] as TextBlock).Text == "L")
            {
                textBlock_Bottom.Text = " LOST            MSG";
            }
            else
            {
                textBlock_Bottom.Text = "";
            }
        }
        //绘制按钮按下效果函数
        public void Draw_My_Button(int i_State, int i)
        {
             //按钮按下效果图片初始为全部隐藏
                image1.Visibility = Visibility.Collapsed;
                image2.Visibility = Visibility.Collapsed;
                image3.Visibility = Visibility.Collapsed;
           
                image6.Visibility = Visibility.Collapsed;
                image7.Visibility = Visibility.Collapsed;
                image8.Visibility = Visibility.Collapsed;
                image9.Visibility = Visibility.Collapsed;
                image10.Visibility = Visibility.Collapsed;
                image12.Visibility = Visibility.Collapsed;

                image1_ON.Visibility = Visibility.Collapsed;
                image2_ON.Visibility = Visibility.Collapsed;
                image3_ON.Visibility = Visibility.Collapsed;

                if (m_bIsShowDoubleHand == false)
                {
                    image4.Visibility = Visibility.Collapsed;
                    image5.Visibility = Visibility.Collapsed;
                    image4_ON.Visibility = Visibility.Collapsed;
                    image5_ON.Visibility = Visibility.Collapsed;
                }
                else if (m_bIsShowDoubleHand == true)
                {
                    if (image_HandPow.Visibility == Visibility.Visible)
                    {
                        image5.Visibility = Visibility.Collapsed;
                        image5_ON.Visibility = Visibility.Collapsed;
                    }
                    else if (image_HandOFF.Visibility == Visibility.Visible)
                    {
                        image4.Visibility = Visibility.Collapsed;
                        image4_ON.Visibility = Visibility.Collapsed;
                    }
                  
                }
                
            //若当前有按钮被按下
            if (i_State == 0 )
            {
                switch (i)
                {
                    case 1://1键被按下
                        {
                            if (m_iFunOrder == 0)
                            {
                                image1.Visibility = Visibility.Visible;
                                break;
                            }
                            else
                            {
                                image1_ON.Visibility = Visibility.Visible;
                                break;
                            }
                        }
                    case 2://2键被按下
                        {
                            if (m_iFunOrder == 0)
                            {
                                image2.Visibility = Visibility.Visible;
                                break;
                            }
                            else
                            {
                                image2_ON.Visibility = Visibility.Visible;
                                break;
                            }

                        }
                    case 3://3键被按下
                        {
                            if (m_iFunOrder == 0)
                            {
                                image3.Visibility = Visibility.Visible;
                                break;
                            }
                            else
                            {
                                image3_ON.Visibility = Visibility.Visible;
                                break;
                            }
                        }
                    case 4://4键被按下
                        {
                            if (m_iFunOrder == 0)
                            {
                                image4.Visibility = Visibility.Visible;
                                break;
                            }
                            else
                            {
                                image4_ON.Visibility = Visibility.Visible;
                                break;
                            }
                        }
                    case 5://5键被按下
                        {
                            if (m_iFunOrder == 0)
                            {
                                image5.Visibility = Visibility.Visible;
                                break;
                            }
                            else
                            {
                                image5_ON.Visibility = Visibility.Visible;
                                break;
                            }
                        }
                    case 6://6键被按下
                        {
                            image6.Visibility = Visibility.Visible;
                            break;
                        }
                    case 7://7键被按下
                        {
                            image7.Visibility = Visibility.Visible;
                            break;
                        }
                    case 8://8键被按下
                        {
                            image8.Visibility = Visibility.Visible;
                            break;
                        }
                    case 9://9键被按下
                        {
                            image9.Visibility = Visibility.Visible;
                            break;
                        }
                    case 10://10键被按下
                        {
                            image10.Visibility = Visibility.Visible;
                            break;
                        }
                    case 12://12键被按下
                        {
                            image12.Visibility = Visibility.Visible;
                            break;
                        }
                    default: break;
                }

            }
        }
        //播放按键音函数
        private void Button_Sound()
        {
            if (m_bSound == true && m_iFunOrder != 0)
            {
                SoundPlayer player = new SoundPlayer();
                player.SoundLocation = @"Button.WAV";
                player.Load();
                player.Play();
            }
        }
        bool m_Warnsound = true;
        //播放报警音函数
        private void Warn_Sound()
        {
         
                SoundPlayer player1 = new SoundPlayer();
                player1.SoundLocation = @"Warn.WAV";
                if (m_Warnsound == true)
                {
                    player1.Load();
                    player1.Play();
                }
                else
                {
                    player1.Stop();
                    m_Warnsound = true;
                }                   
        }

        //初始化各种信息函数（关机时才会调用）
        public void Initial_My_Struct()
        {
                //逻辑变量初始化
                 m_Count_Choose = 0;
                 m_Count_Choose_Down = 15;
                 m_Count_Choose_Up = 0;
                 m_iBRG_Disp = 0;
                 m_iSORT_Disp = 0;
                 m_iNAME_Disp = 0;
                 m_iSelect = 0;
                 m_iList_Select = 0;
                 m_Detail_Choose = 0;                    
                 m_bOwnposdisp = false;
                 //控件显示及隐藏
                Grid_1.Visibility = Visibility.Collapsed;
                Grid_2.Visibility = Visibility.Collapsed;
                Grid_2_3_5_6.Visibility = Visibility.Collapsed;
                Grid_3.Visibility = Visibility.Collapsed;
                Grid_4.Visibility = Visibility.Collapsed;
                Grid_5.Visibility = Visibility.Collapsed;
                Grid_6.Visibility = Visibility.Collapsed;
              
                rectangle_Title.Visibility = Visibility.Collapsed;
                rectangle_MenuTitleLeft.Visibility = Visibility.Collapsed;
                rectangle_MenuTitleRight.Visibility = Visibility.Collapsed;
              
                rectangle_Total.Visibility = Visibility.Collapsed;
                rectangle_Bottom.Visibility = Visibility.Collapsed;
               
                textBlock_Title.Visibility = Visibility.Collapsed;
                textBlock_MenuTitleLeft.Visibility = Visibility.Collapsed;
                textBlock_MenuTitleMid.Visibility = Visibility.Collapsed;
                textBlock_MenuTitleRight.Visibility = Visibility.Collapsed;
                textBlock_Time.Visibility = Visibility.Collapsed;
                textBlock_Total.Visibility = Visibility.Collapsed;
                textBlock_Bottom.Visibility = Visibility.Collapsed;
              
                rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Up.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                
                image_ON.Visibility = Visibility.Collapsed;                                               
            //屏幕静态显示内容
            m_Struct_No1.Menu_Title_Left = "  BRG :";
            m_Struct_No1.Menu_Title_Right = "NAME / MMSI";
            m_Struct_No1.Menu_Title_Mid = "  RNG";
             
            
        }
        //用于控制仪器密码的参数，密码从debug文件夹数据库，Ownship表中读取
        string m_strPassCode = "";
        string m_strPassCodeinPut = "";
        //初始化本船信息函数，存到数组m_strOwnData中
        private void initOwnship()
        {
            //连接数据库
            string strConnection = "Provider = Microsoft.ACE.OLEDB.12.0;";
            strConnection += @"Data Source =JHS182Data.mdb ";
            //初始化所需数据
            string strNAME = "";
            string strMMSI = "";
            string strCALL_SIGH = "";
            string strIMO_NO = "";
            string strNav_Status = "";
            string strPos_Accuracy = "";
            double dLAT = 0;
            double dLON =0;
            float fCOG = 0;
            int iHDG = 0;
            float fROT = 0;
            string strDestination = "";
            string strArival_Date = "";
            string strArival_Time = "";
            int iLength = 0;
            int iBeam = 0;
            float fDraught = 0;
            string strShip_Type = "";
            string strCargo_Type = "";
            float fSOG = 0;
            using (OleDbConnection objConnection = new OleDbConnection(strConnection))
            {
                //打开连接
                objConnection.Open();
                //sql语句   
                OleDbCommand sqlcmd = new OleDbCommand(@"select * from Ownship where ID=" + 1, objConnection);
                //执行查询，用using替代reader.Close()  
                using (OleDbDataReader reader = sqlcmd.ExecuteReader())  
                {
                    if (reader.Read())
                    {
                        //取得字段的值  
                        strNAME = reader["AIS NAME"].ToString();
                        strMMSI = reader["MMSI"].ToString();
                        strCALL_SIGH = reader["CALL SIGN"].ToString();
                        strIMO_NO = reader["IMO NO"].ToString();
                        strNav_Status = reader["NAV STATUS"].ToString();
                        strPos_Accuracy = reader["PA"].ToString();
                        dLAT = (double)reader["LAT"];
                        dLON = (double)reader["LON"];
                        fCOG = (float)reader["COG"];
                        iHDG = (int)reader["HDG"];
                        fROT = (float)reader["ROT"];
                        strDestination = reader["DESTINATION"].ToString();
                        strArival_Date = reader["ARRIVAL DATE"].ToString();
                        strArival_Time = reader["ARRIVAL TIME"].ToString();
                        iLength = (int)reader["LENGTH"];
                        iBeam = (int)reader["BEAM"];
                        fDraught = (float)reader["DRAUGHT"];
                        strShip_Type = reader["TYPE DETAIL1"].ToString();
                        fSOG = (float)reader["SOG"];
                        strCargo_Type = reader["TYPE DETAIL2"].ToString();
                        m_strPassCode = reader["PASSCODE"].ToString();
                    }

                }
                //将实时计算所需信息存入各个数组
                m_dOwnLAT = dLAT;
                m_dOwnLON = dLON;
                m_fOwnCOG = fCOG;
                m_fOwnSOG = fSOG;
                m_fOwnROT = fROT;
                m_iOwnHDG = iHDG;
                m_fOwnDraught = fDraught;
                //将数据存入数组,便于显示
                m_strOwnshipData[0] = strNAME; 
                m_strOwnshipData[1] = strMMSI;
                m_strOwnshipData[2] = strCALL_SIGH;
                m_strOwnshipData[3] = strIMO_NO;
                m_strOwnshipData[4] = strNav_Status;
                m_strOwnshipData[5] = strPos_Accuracy;
                m_strOwnshipData[6] = ((int)(dLAT / 60)).ToString("000") + "°";
                m_strOwnshipData[7] = ((int)(dLON / 60)).ToString("000") + "°";
                m_strOwnshipData[8] = fCOG.ToString();
                m_strOwnshipData[9] = iHDG.ToString();
                m_strOwnshipData[10] = fROT.ToString();
                m_strOwnshipData[11] = strDestination;
                m_strOwnshipData[12] = strArival_Date;
                m_strOwnshipData[13] = strArival_Time;
                m_strOwnshipData[14] = iLength.ToString();
                m_strOwnshipData[15] = iBeam.ToString();
                m_strOwnshipData[16] = fDraught.ToString();
                m_strOwnshipData[17] = strShip_Type;
                m_strOwnshipData[18] = strCargo_Type;
                m_strOwnshipData[19] = fSOG.ToString();
                m_strOwnshipData[20] = (dLAT % 60).ToString("0.0000");
                m_strOwnshipData[21] = (dLON % 60).ToString("0.0000");


            }
        }
        //初始化其他船舶信息函数，存到数组m_strData中
        private void getConn(int ID)
        {
            //连接数据库
            string strConnection = "Provider = Microsoft.ACE.OLEDB.12.0;";
            strConnection += @"Data Source =JHS182Data.mdb ";
            //初始化所需数据
            double dBRG = 0;
            double dRNG = 0;
            string strNAME = "";
            string strMMSI = "";
            string strCALL_SIGH = "";
            double dCPA = 0;
            int iTCPA = 0;
            string strIMO_NO = "";
            string strNav_Status = " ";
            string strPos_Accuracy = " ";
            double dLAT  = 0;
            double dLON = 0;
            float fCOG =0;
            int iHDG = 0;
            float fROT = 0;
            string strDestination = "";
            string strArival_Date = "";
            string strArival_Time = "";
            int iLength = 0;
            int iBeam = 0;
            float fDraught = 0;
            string strShip_Type = "";
            string strCargo_Type = "";
            float fSOG = 0;
           
            using (OleDbConnection objConnection = new OleDbConnection(strConnection))
            {
                //打开连接
                objConnection.Open();
                //sql语句  
                OleDbCommand sqlcmd = new OleDbCommand(@"select * from Target where ID=" + ID, objConnection);
                //执行查询，用using替代reader.Close()  
                using (OleDbDataReader reader = sqlcmd.ExecuteReader()) 
                {
                    if (reader.Read())
                    {
                        //取得字段的值  
                        dBRG =  (double)reader["BRG"];  
                        dRNG = (double)reader["RNG"];
                        strNAME = reader["AIS NAME"].ToString();
                        strMMSI = reader["MMSI"].ToString();
                        strCALL_SIGH = reader["CALL SIGN"].ToString();
                        dCPA = (double)reader["CPA"];
                        iTCPA = int.Parse(reader["TCPA"].ToString());
                        strIMO_NO = reader["IMO NO"].ToString();
                        strNav_Status = reader["NAV STATUS"].ToString();
                        strPos_Accuracy = reader["PA"].ToString();
                        dLAT = (double)reader["LAT"];
                        dLON = (double)reader["LON"];
                        fCOG = (float)reader["COG"];
                        iHDG = (int)reader["HDG"];
                        fROT = (float)reader["ROT"];
                        strDestination = reader["DESTINATION"].ToString();
                        strArival_Date = reader["ARRIVAL DATE"].ToString();
                        strArival_Time = reader["ARRIVAL TIME"].ToString();
                        iLength = (int)reader["LENGTH"];
                        iBeam = (int)reader["BEAM"];
                        fDraught = (float)reader["DRAUGHT"];
                        strShip_Type = reader["TYPE DETAIL1"].ToString();
                        fSOG = (float)reader["SOG"];
                        strCargo_Type = reader["TYPE DETAIL2"].ToString();
                    }
                    
               }
                //将实时计算所需信息存入各个数组
                   m_dLAT[ID - 1] = dLAT;
                   m_dLON[ID - 1] = dLON;
                   m_fCOG[ID - 1] = fCOG;
                   m_fSOG[ID - 1] = fSOG;
                   m_fROT[ID - 1] = fROT;
                   m_iHDG[ID - 1] = iHDG;
                   m_dRNG[ID - 1] = dRNG;
                   m_dBRG[ID - 1] = dBRG;
                   //按行将数据存入数组
                    m_strData[ID - 1, 0] = ((int)dBRG).ToString();
                    m_strData[ID - 1, 1] = dRNG.ToString("0.00");
                    m_strData[ID - 1,2] = strNAME;
                    m_strData[ID - 1,3] = strMMSI;
                    m_strData[ID - 1,4] = strCALL_SIGH;
                    m_strData[ID - 1, 5] = dCPA.ToString("0.0");
                    m_strData[ID - 1, 6] = iTCPA.ToString("0.0");
                    m_strData[ID - 1,7] = strIMO_NO;
                    m_strData[ID - 1, 8] = strNav_Status;
                    m_strData[ID - 1, 9] = strPos_Accuracy;
                    m_strData[ID - 1, 10] = ((int)(dLAT / 60)).ToString("000") + "° " + (dLAT % 60).ToString("0.0000");
                    m_strData[ID - 1, 11] = ((int)(dLON / 60)).ToString("000") + "° " + (dLON % 60).ToString("0.0000"); 
                    m_strData[ID - 1, 12] = fCOG.ToString();
                    m_strData[ID - 1, 13] = iHDG.ToString();
                    m_strData[ID - 1, 14] = fROT.ToString();
                    m_strData[ID - 1, 15] = strDestination;
                    m_strData[ID - 1, 16] = strArival_Date;
                    m_strData[ID - 1, 17] = strArival_Time;
                    m_strData[ID - 1, 18] = iLength.ToString();
                    m_strData[ID - 1, 19] = iBeam.ToString();
                    m_strData[ID - 1, 20] = fDraught.ToString();
                    m_strData[ID - 1, 21] = strShip_Type;
                    m_strData[ID - 1, 22] = fSOG.ToString();
                    m_strData[ID - 1, 23] = strCargo_Type;

            }
        }
        //将m_strData（其他船舶）或m_strOwnData（本船）数据显示在各个界面中，同时用于屏幕实时显示（3秒一次）
        private void Data_Display()
        {

            if (m_Count_Choose != 0)
            {
                textBlock_Total.Text = "  TOTALL:" + m_iTotal.ToString() + " CURSOR:  1";
            }
            else
            {
                textBlock_Total.Text = "  TOTALL:" + m_iTotal.ToString() + " CURSOR:  0";
            }
            switch (m_iFunOrder)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        //控制主界面显示本船信息 ，参数m_bOwnposdisp
                        if (m_bOwnposdisp == false)
                        {
                            //控制显示MMSI或船舶名字 ，参数m_iNAME_Disp
                            if (m_iNAME_Disp == 0)
                            {
                                for (int i = 0; i < 16; i++)
                                {
                                    //主界面数据显示
                                    (stackPanel_1_Left.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 0] + "°:";
                                    (stackPanel_1_Mid.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 1] + "NM";
                                    (stackPanel_1_Right.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 2];
                                    if (m_dRNG[i] < m_fGuardRNG)
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "G";
                                    }
                                    else if (m_dRNG[i]>m_fLostRNG)
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                    }
                                    else
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "";
                                    }
                                    
                                    
                                }
                            }
                            else if (m_iNAME_Disp == 1)
                            {
                                for (int i = 0; i < 16; i++)
                                {
                                    (stackPanel_1_Left.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 0] + "°:";
                                    (stackPanel_1_Mid.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 1] + "NM";
                                    (stackPanel_1_Right.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 3];
                                    if (m_dRNG[i] < m_fGuardRNG)
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "G";
                                    }
                                    else if (m_dRNG[i] > m_fLostRNG)
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                    }
                                    else
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "";
                                    }
                                }
                            }
                           
                        }
                        if (m_bOwnposdisp == true)
                        {
                            if (m_iNAME_Disp == 0)
                            {
                                for (int i = 0; i < 14; i++)
                                {
                                    (stackPanel_1_Left.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 0] + "°:";
                                    (stackPanel_1_Mid.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 1] + "NM";
                                    (stackPanel_1_Right.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 2];
                                    if (m_dRNG[i] < m_fGuardRNG)
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "G";
                                    }
                                    else if (m_dRNG[i] > m_fLostRNG)
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                    }
                                    else
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "";
                                    }
                                }
                            }
                            else if (m_iNAME_Disp == 1)
                            {
                                for (int i = 0; i < 14; i++)
                                {
                                    (stackPanel_1_Left.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 0] + "°:";
                                    (stackPanel_1_Mid.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 1] + "NM";
                                    (stackPanel_1_Right.Children[i] as TextBlock).Text = m_strData[i + m_Count_Choose_Up, 3];
                                    if (m_dRNG[i] < m_fGuardRNG)
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "G";
                                    }
                                    else if (m_dRNG[i] > m_fLostRNG)
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                    }
                                    else
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "";
                                    }
                                }
                            }
                            //本船主界面信息显示（一直保持高亮）
                            (stackPanel_1_Guard.Children[14] as TextBlock).Text = "";
                            (stackPanel_1_Guard.Children[15] as TextBlock).Text = "";
                            (stackPanel_1_Left.Children[14] as TextBlock).Text = "N "+m_strOwnshipData[6];
                            (stackPanel_1_Mid.Children[14] as TextBlock).Text =m_strOwnshipData[20];
                            (stackPanel_1_Right.Children[14] as TextBlock).Text = "SOG    " + m_strOwnshipData[19] + "KT";
                            (stackPanel_1_Left.Children[15] as TextBlock).Text = "E " + m_strOwnshipData[7];
                            (stackPanel_1_Mid.Children[15] as TextBlock).Text = m_strOwnshipData[21];
                            (stackPanel_1_Right.Children[15] as TextBlock).Text = "COG    " + m_strOwnshipData[8] +"°";
                            (stackPanel_1_Left.Children[14] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Right.Children[14] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Mid.Children[14] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Guard.Children[14] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Left.Children[14] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Right.Children[14] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Mid.Children[14] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Guard.Children[14] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); 
                            (stackPanel_1_Left.Children[15] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Right.Children[15] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Mid.Children[15] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Guard.Children[15] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Left.Children[15] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Right.Children[15] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Mid.Children[15] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Guard.Children[15] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); 
                        }
                        if (m_Count_Choose != 0)
                        {
                            textBlock_Total.Text = "  TOTALL:" + m_iTotal.ToString() + " CURSOR:  1";
                        }
                        else
                        {
                            textBlock_Total.Text = "  TOTALL:" + m_iTotal.ToString() + " CURSOR:  0";
                        }
                        break;
                    }
                 //显示内容都为三个离本船最近其他船舶
                case 2:
                case 5:
                case 6:
                    {
                        if (m_iNAME_Disp == 0)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                (stackPanel_2_3_Left.Children[i] as TextBlock).Text = m_strData[i, 0] + "°:";
                                (stackPanel_2_3_Mid.Children[i] as TextBlock).Text = m_strData[i, 1] + "NM";
                                (stackPanel_2_3_Right.Children[i] as TextBlock).Text = m_strData[i, 2];
                                if (m_dRNG[i] < m_fGuardRNG)
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "G";
                                }
                                else if (m_dRNG[i] > m_fLostRNG)
                                {
                                    (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                }
                                else
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "";
                                }
                            }
                        }
                        else if (m_iNAME_Disp == 1)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                (stackPanel_2_3_Left.Children[i] as TextBlock).Text = m_strData[i, 0] + "°:";
                                (stackPanel_2_3_Mid.Children[i] as TextBlock).Text = m_strData[i, 1] + "NM";
                                (stackPanel_2_3_Right.Children[i] as TextBlock).Text = m_strData[i, 3];
                                if (m_dRNG[i] < m_fGuardRNG)
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "G";
                                }
                                else if (m_dRNG[i] > m_fLostRNG)
                                {
                                    (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                }
                                else
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "";
                                }
                            }
                        }
                        break;
                    }
                   //其他船舶详细信息显示
                case 3:
                    {
                        if (m_iNAME_Disp == 0)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                (stackPanel_2_3_Left.Children[i] as TextBlock).Text = m_strData[i, 0] + "°:";
                                (stackPanel_2_3_Mid.Children[i] as TextBlock).Text = m_strData[i, 1] + "NM";
                                (stackPanel_2_3_Right.Children[i] as TextBlock).Text = m_strData[i, 2];
                                if (m_dRNG[i] < m_fGuardRNG)
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "G";
                                }
                                else if (m_dRNG[i] > m_fLostRNG)
                                {
                                    (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                }
                                else
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "";
                                }
                            }
                        }
                        else if (m_iNAME_Disp == 1)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                (stackPanel_2_3_Left.Children[i] as TextBlock).Text = m_strData[i, 0] + "°:";
                                (stackPanel_2_3_Mid.Children[i] as TextBlock).Text = m_strData[i, 1] + "NM";
                                (stackPanel_2_3_Right.Children[i] as TextBlock).Text = m_strData[i, 3];
                                if (m_dRNG[i] < m_fGuardRNG)
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "G";
                                }
                                else if (m_dRNG[i] > m_fLostRNG)
                                {
                                    (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                }
                                else
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "";
                                }
                            }
                        }

                        if (m_Detail_Choose == 0)
                        {
                            Grid_DetailSel.Visibility = Visibility.Collapsed;
                            polygon_3_ChoseSign_Up.Visibility = Visibility.Collapsed;
                            polygon_3_ChoseSign_Down.Visibility = Visibility.Visible;
                            (stackPanel_3_Detile.Children[0] as TextBlock).Text = "NAME :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 2];
                            (stackPanel_3_Detile.Children[1] as TextBlock).Text = "MMSI : " + m_strData[m_Count_Choose + m_Count_Choose_Up, 3];
                            (stackPanel_3_Detile.Children[2] as TextBlock).Text = "CALL SIGN:" + m_strData[m_Count_Choose + m_Count_Choose_Up, 4];
                            (stackPanel_3_Detile.Children[3] as TextBlock).Text = "IMO NO.  :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 7];
                            (stackPanel_3_Detile.Children[4] as TextBlock).Text = "CPA      :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 5] + "NM ";
                            (stackPanel_3_Detile.Children[5] as TextBlock).Text = "TCPA     :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 6] + "MIN ";
                            (stackPanel_3_Detile.Children[6] as TextBlock).Text = "BEARING  :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 0] + "° ";
                            (stackPanel_3_Detile.Children[7] as TextBlock).Text = "RANGE    :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 1] + "NM ";
                            (stackPanel_3_Detile.Children[8] as TextBlock).Text = "NAVIGATIONAL STATUS:";
                            (stackPanel_3_Detile.Children[9] as TextBlock).Text = m_strData[m_Count_Choose + m_Count_Choose_Up, 8];
                            (stackPanel_3_Detile.Children[10] as TextBlock).Text = "POSITION(POS) SENSOR:";
                            (stackPanel_3_Detile.Children[11] as TextBlock).Text = " INTEGRATED ";
                            (stackPanel_3_Detile.Children[12] as TextBlock).Text = "POSITION ACCURACY:" + m_strData[m_Count_Choose + m_Count_Choose_Up, 9];
                            (stackPanel_3_Detile.Children[13] as TextBlock).Text = "";
                        }
                        if (m_Detail_Choose == 1)
                        {
                            Grid_DetailSel.Visibility = Visibility.Collapsed;
                            polygon_3_ChoseSign_Up.Visibility = Visibility.Visible;
                            polygon_3_ChoseSign_Down.Visibility = Visibility.Visible;
                            (stackPanel_3_Detile.Children[0] as TextBlock).Text = "" ;
                            (stackPanel_3_Detile.Children[1] as TextBlock).Text = "POS    :N " + m_strData[m_Count_Choose + m_Count_Choose_Up, 10];
                            (stackPanel_3_Detile.Children[2] as TextBlock).Text = "       :E" + m_strData[m_Count_Choose + m_Count_Choose_Up, 11];
                            (stackPanel_3_Detile.Children[3] as TextBlock).Text = "COG    :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 12] + "  ° ";
                            (stackPanel_3_Detile.Children[4] as TextBlock).Text = "SOG    :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 22]+ "KT ";
                            (stackPanel_3_Detile.Children[5] as TextBlock).Text = "HDG    :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 13] + "  ° ";
                            (stackPanel_3_Detile.Children[6] as TextBlock).Text = "ROT    :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 14] + " °MIN ";
                            (stackPanel_3_Detile.Children[7] as TextBlock).Text = "DESTINATION:";
                            (stackPanel_3_Detile.Children[8] as TextBlock).Text = "  "+ m_strData[m_Count_Choose + m_Count_Choose_Up, 15] ;
                            (stackPanel_3_Detile.Children[9] as TextBlock).Text = "ETA    :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 16] + " " + m_strData[m_Count_Choose + m_Count_Choose_Up, 17];
                            (stackPanel_3_Detile.Children[10] as TextBlock).Text = "LENGTH :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 18] + "M";
                            (stackPanel_3_Detile.Children[11] as TextBlock).Text = "BEAM   :" + m_strData[m_Count_Choose + m_Count_Choose_Up, 19] + "M";
                            (stackPanel_3_Detile.Children[12] as TextBlock).Text = "DRAUGHT:" + m_strData[m_Count_Choose + m_Count_Choose_Up, 20] + "M";
                            (stackPanel_3_Detile.Children[13] as TextBlock).Text = "";
                          
                        }
                        if (m_Detail_Choose == 2)
                        {
                            Grid_DetailSel.Visibility = Visibility.Visible;
                            (Wrappanel_DetailSel.Children[0] as TextBlock).Text = "[EXIT]";
                            (Wrappanel_DetailSel.Children[1] as TextBlock).Text = "[EDIT AND TX]";
                            (Wrappanel_DetailSel.Children[2] as TextBlock).Text = "[INTERROGATION]";
                            (Wrappanel_DetailSel.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                            (Wrappanel_DetailSel.Children[1] as TextBlock).Foreground = Brushes.Black;
                            (Wrappanel_DetailSel.Children[2] as TextBlock).Foreground = Brushes.Black;
                            (Wrappanel_DetailSel.Children[0] as TextBlock).Background = Brushes.Black;
                            (Wrappanel_DetailSel.Children[1] as TextBlock).Background = Brushes.Transparent;
                            (Wrappanel_DetailSel.Children[2] as TextBlock).Background = Brushes.Transparent;

                            polygon_3_ChoseSign_Up.Visibility = Visibility.Visible;
                            polygon_3_ChoseSign_Down.Visibility = Visibility.Collapsed;
                            (stackPanel_3_Detile.Children[0] as TextBlock).Text = "";
                            (stackPanel_3_Detile.Children[1] as TextBlock).Text = "SHIP TYPE:" ;
                            (stackPanel_3_Detile.Children[2] as TextBlock).Text =m_strData[m_Count_Choose + m_Count_Choose_Up, 21];
                            (stackPanel_3_Detile.Children[3] as TextBlock).Text = "CARGO TYPE:";
                            (stackPanel_3_Detile.Children[4] as TextBlock).Text = m_strData[m_Count_Choose + m_Count_Choose_Up, 23];
                            (stackPanel_3_Detile.Children[5] as TextBlock).Text = "CLASS    : CLASS A";
                            (stackPanel_3_Detile.Children[6] as TextBlock).Text = "";
                            (stackPanel_3_Detile.Children[7] as TextBlock).Text = "";
                            (stackPanel_3_Detile.Children[8] as TextBlock).Text = "";
                            (stackPanel_3_Detile.Children[9] as TextBlock).Text = "";
                            (stackPanel_3_Detile.Children[10] as TextBlock).Text = "";
                            (stackPanel_3_Detile.Children[11] as TextBlock).Text = "";
                            (stackPanel_3_Detile.Children[12] as TextBlock).Text = "";
                            (stackPanel_3_Detile.Children[13] as TextBlock).Text = "";
                        }
                        if (m_Detail_Choose == 3)
                        {
                            (Wrappanel_DetailSel.Children[1] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                            (Wrappanel_DetailSel.Children[0] as TextBlock).Foreground = Brushes.Black;
                            (Wrappanel_DetailSel.Children[2] as TextBlock).Foreground = Brushes.Black;
                            (Wrappanel_DetailSel.Children[1] as TextBlock).Background = Brushes.Black;
                            (Wrappanel_DetailSel.Children[0] as TextBlock).Background = Brushes.Transparent;
                            (Wrappanel_DetailSel.Children[2] as TextBlock).Background = Brushes.Transparent;
                        }
                        if (m_Detail_Choose == 4)
                        {
                            (Wrappanel_DetailSel.Children[2] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                            (Wrappanel_DetailSel.Children[0] as TextBlock).Foreground = Brushes.Black;
                            (Wrappanel_DetailSel.Children[1] as TextBlock).Foreground = Brushes.Black;
                            (Wrappanel_DetailSel.Children[2] as TextBlock).Background = Brushes.Black;
                            (Wrappanel_DetailSel.Children[0] as TextBlock).Background = Brushes.Transparent;
                            (Wrappanel_DetailSel.Children[1] as TextBlock).Background = Brushes.Transparent;
                        }
                        break;
                    }

                    //设置菜单船舶信息显示
                case 4:
                    {
                        polygon_4_ChoseSign_Down.Visibility = Visibility.Visible;
                        polygon_4_ChoseSign_Up.Visibility = Visibility.Collapsed;
                        if (m_iNAME_Disp == 0)
                        {
                            for (int i = 0; i < 14; i++)
                            {
                                (stackPanel_4_Left.Children[i] as TextBlock).Text = m_strData[i, 0] + "°:";
                                (stackPanel_4_Mid.Children[i] as TextBlock).Text = m_strData[i, 1] + "NM";
                                (stackPanel_4_Right.Children[i] as TextBlock).Text = m_strData[i, 2];
                                if (m_dRNG[i] < m_fGuardRNG)
                                {
                                    (stackPanel_4_Guard.Children[i] as TextBlock).Text = "G";
                                }
                                else if (m_dRNG[i] > m_fLostRNG)
                                {
                                    (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                }
                                else
                                {
                                    (stackPanel_4_Guard.Children[i] as TextBlock).Text = "";
                                }

                            }
                        }
                        else if (m_iNAME_Disp == 1)
                        {
                            for (int i = 0; i < 14; i++)
                            {
                                (stackPanel_4_Left.Children[i] as TextBlock).Text = m_strData[i, 0] + "°:";
                                (stackPanel_4_Mid.Children[i] as TextBlock).Text = m_strData[i, 1] + "NM";
                                (stackPanel_4_Right.Children[i] as TextBlock).Text = m_strData[i, 3];
                                if (m_dRNG[i] < m_fGuardRNG)
                                {
                                    (stackPanel_4_Guard.Children[i] as TextBlock).Text = "G";
                                }
                                else if (m_dRNG[i] > m_fLostRNG)
                                {
                                    (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                }
                                else
                                {
                                    (stackPanel_4_Guard.Children[i] as TextBlock).Text = "";
                                }
                            }
                        }
                        if (m_iSelect == 4)
                        {
                            polygon_4_ChoseSign_Up.Visibility = Visibility.Visible;
                            polygon_4_ChoseSign_Down.Visibility = Visibility.Collapsed;
                            if (m_iNAME_Disp == 0)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    (stackPanel_4_Left.Children[i] as TextBlock).Text = m_strData[i + 14, 0] + "°:";
                                    (stackPanel_4_Mid.Children[i] as TextBlock).Text = m_strData[i + 14, 1] + "NM";
                                    (stackPanel_4_Right.Children[i] as TextBlock).Text = m_strData[i + 14, 2];

                                    if (m_dRNG[i + 14] < m_fGuardRNG)
                                    {
                                        (stackPanel_4_Guard.Children[i] as TextBlock).Text = "G";
                                    }
                                    else if (m_dRNG[i+14] > m_fLostRNG)
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                    }
                                    else
                                    {
                                        (stackPanel_4_Guard.Children[i] as TextBlock).Text = "";
                                    }
                                    
                                    
                                }
                                for (int i = 6; i < 14; i++)
                                {
                                    (stackPanel_4_Left.Children[i] as TextBlock).Text = m_strData[i + 14, 0];
                                    (stackPanel_4_Mid.Children[i] as TextBlock).Text = m_strData[i + 14, 1];
                                    (stackPanel_4_Right.Children[i] as TextBlock).Text = m_strData[i + 14, 2];
                              
                                        (stackPanel_4_Guard.Children[i] as TextBlock).Text = "";
                                    
                                   
                                }
                            }
                            else if (m_iNAME_Disp == 1)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    (stackPanel_4_Left.Children[i] as TextBlock).Text = m_strData[i + 14, 0] + "°:";
                                    (stackPanel_4_Mid.Children[i] as TextBlock).Text = m_strData[i + 14, 1] + "NM";
                                    (stackPanel_4_Right.Children[i] as TextBlock).Text = m_strData[i + 14, 3];

                                    if (m_dRNG[i + 14] < m_fGuardRNG)
                                    {
                                        (stackPanel_4_Guard.Children[i] as TextBlock).Text = "G";
                                    }
                                    else if (m_dRNG[i+14] > m_fLostRNG)
                                    {
                                        (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                    }
                                    else
                                    {
                                        (stackPanel_4_Guard.Children[i] as TextBlock).Text = "";
                                    }
                                    
                                    
                                }
                                for (int i = 6; i < 14; i++)
                                {
                                    (stackPanel_4_Left.Children[i] as TextBlock).Text = m_strData[i + 14, 0];
                                    (stackPanel_4_Mid.Children[i] as TextBlock).Text = m_strData[i + 14, 1];
                                    (stackPanel_4_Right.Children[i] as TextBlock).Text = m_strData[i + 14, 3];
                               
                                        (stackPanel_4_Guard.Children[i] as TextBlock).Text = "";
                                    
                                    
                                }
                            }
                        }                   
                            break;
                        }
                 //本船详细信息显示
                case 43:
                    {
                        if (m_iNAME_Disp == 0)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                (stackPanel_2_3_Left.Children[i] as TextBlock).Text = m_strData[i, 0] + "°:";
                                (stackPanel_2_3_Mid.Children[i] as TextBlock).Text = m_strData[i, 1] + "NM";
                                (stackPanel_2_3_Right.Children[i] as TextBlock).Text = m_strData[i, 2];
                                if (m_dRNG[i] < m_fGuardRNG)
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "G";
                                }
                                else if (m_dRNG[i] > m_fLostRNG)
                                {
                                    (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                }
                                else
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "";
                                }
                            }
                        }
                        else if (m_iNAME_Disp == 1)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                (stackPanel_2_3_Left.Children[i] as TextBlock).Text = m_strData[i, 0] + "°:";
                                (stackPanel_2_3_Mid.Children[i] as TextBlock).Text = m_strData[i, 1] + "NM";
                                (stackPanel_2_3_Right.Children[i] as TextBlock).Text = m_strData[i, 3];
                                if (m_dRNG[i] < m_fGuardRNG)
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "G";
                                }
                                else if (m_dRNG[i] > m_fLostRNG)
                                {
                                    (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                }
                                else
                                {
                                    (stackPanel_2_3_Guard.Children[i] as TextBlock).Text = "";
                                }
                            }
                        }

                        if (m_Detail_Choose == 0)
                        {
                            polygon_3_ChoseSign_Up.Visibility = Visibility.Collapsed;
                            polygon_3_ChoseSign_Down.Visibility = Visibility.Visible;
                            (stackPanel_3_Detile.Children[0] as TextBlock).Text = "NAME :" + m_strOwnshipData[0];
                            (stackPanel_3_Detile.Children[1] as TextBlock).Text = "MMSI : " + m_strOwnshipData[1];
                            (stackPanel_3_Detile.Children[2] as TextBlock).Text = "CALL SIGN:" + m_strOwnshipData[2];
                            (stackPanel_3_Detile.Children[3] as TextBlock).Text = "IMO NO.  :" + m_strOwnshipData[3];
                            (stackPanel_3_Detile.Children[4] as TextBlock).Text = "NAVIGATIONAL STATUS:";
                            (stackPanel_3_Detile.Children[5] as TextBlock).Text = m_strOwnshipData[4];
                            (stackPanel_3_Detile.Children[6] as TextBlock).Text = "POSITION(POS) SENSOR:";
                            (stackPanel_3_Detile.Children[7] as TextBlock).Text = " INTEGRATED ";
                            (stackPanel_3_Detile.Children[8] as TextBlock).Text = "POSITION ACCURACY:" + m_strOwnshipData[5];
                            (stackPanel_3_Detile.Children[9] as TextBlock).Text = "POS    :N " + m_strOwnshipData[6]+m_strOwnshipData[20];
                            (stackPanel_3_Detile.Children[10] as TextBlock).Text = "       :E" + m_strOwnshipData[7] + m_strOwnshipData[21];
                            (stackPanel_3_Detile.Children[11] as TextBlock).Text = "COG    :" + m_strOwnshipData[8] +"  ° ";
                            (stackPanel_3_Detile.Children[12] as TextBlock).Text = "SOG    :" + m_strOwnshipData[19] + "KT ";
                            (stackPanel_3_Detile.Children[13] as TextBlock).Text = "";
                        }
                        if (m_Detail_Choose == 1)
                        {
                            polygon_3_ChoseSign_Up.Visibility = Visibility.Visible;
                            polygon_3_ChoseSign_Down.Visibility = Visibility.Collapsed;
                            (stackPanel_3_Detile.Children[0] as TextBlock).Text = "";
                            (stackPanel_3_Detile.Children[1] as TextBlock).Text = "HDG    :" + m_strOwnshipData[9] + "  ° ";
                            (stackPanel_3_Detile.Children[2] as TextBlock).Text = "ROT    :" + m_strOwnshipData[10] + " °MIN ";
                            (stackPanel_3_Detile.Children[3] as TextBlock).Text = "DESTINATION:";
                            (stackPanel_3_Detile.Children[4] as TextBlock).Text = "  " + m_strOwnshipData[11];
                            (stackPanel_3_Detile.Children[5] as TextBlock).Text = "ETA    :" + m_strOwnshipData[12] + " " + m_strOwnshipData[13];
                            (stackPanel_3_Detile.Children[6] as TextBlock).Text = "LENGTH :" + m_strOwnshipData[14] +"M";
                            (stackPanel_3_Detile.Children[7] as TextBlock).Text = "BEAM   :" + m_strOwnshipData[15] + "M";
                            (stackPanel_3_Detile.Children[8] as TextBlock).Text = "DRAUGHT:" + m_strOwnshipData[16] + "M";
                            (stackPanel_3_Detile.Children[9] as TextBlock).Text = "SHIP TYPE:";
                            (stackPanel_3_Detile.Children[10] as TextBlock).Text = m_strOwnshipData[17];
                            (stackPanel_3_Detile.Children[11] as TextBlock).Text = "CARGO TYPE:";
                            (stackPanel_3_Detile.Children[12] as TextBlock).Text = m_strOwnshipData[18];
                            (stackPanel_3_Detile.Children[13] as TextBlock).Text = "PERSONS ON BOARD:" + m_iPersons.ToString();

                        }                     

                        break;
                    }
               //设置的子菜单，显示内容均与case 4相同
                case 41:
                case 420:
                case 411:
                case 412:
                case 413:           
                    {
                        if (m_iNAME_Disp == 0)
                        {
                            for (int i = 0; i < 14; i++)
                            {
                                (stackPanel_4_Left.Children[i] as TextBlock).Text = m_strData[i, 0] + "°:";
                                (stackPanel_4_Mid.Children[i] as TextBlock).Text = m_strData[i, 1] + "NM";
                                (stackPanel_4_Right.Children[i] as TextBlock).Text = m_strData[i, 2];
                                if (m_dRNG[i] < m_fGuardRNG)
                                {
                                    (stackPanel_4_Guard.Children[i] as TextBlock).Text = "G";
                                }
                                else if (m_dRNG[i] > m_fLostRNG)
                                {
                                    (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                }
                                else
                                {
                                    (stackPanel_4_Guard.Children[i] as TextBlock).Text = "";
                                }

                            }
                        }
                        else if (m_iNAME_Disp == 1)
                        {
                            for (int i = 0; i < 14; i++)
                            {
                                (stackPanel_4_Left.Children[i] as TextBlock).Text = m_strData[i, 0] + "°:";
                                (stackPanel_4_Mid.Children[i] as TextBlock).Text = m_strData[i, 1] + "NM";
                                (stackPanel_4_Right.Children[i] as TextBlock).Text = m_strData[i, 3];
                                if (m_dRNG[i] < m_fGuardRNG)
                                {
                                    (stackPanel_4_Guard.Children[i] as TextBlock).Text = "G";
                                }
                                else if (m_dRNG[i] > m_fLostRNG)
                                {
                                    (stackPanel_1_Guard.Children[i] as TextBlock).Text = "L";
                                }
                                else
                                {
                                    (stackPanel_4_Guard.Children[i] as TextBlock).Text = "";
                                }
                            }
                        }
                        break;
                    }
                    }
        }
        //实时计算船舶信息（3秒一次）
        private void RealTime_Cal(object sender, EventArgs e)
        {
    
            //临时参数
            double dOwnLAT = 0;
            float fTime_ROT = 1/20f;
            m_fOwnCOG += m_fOwnROT * fTime_ROT;
            m_iOwnHDG += (int)(m_fOwnROT * fTime_ROT);
            //实时计算本船经纬度
            dOwnLAT = m_dOwnLAT + m_fOwnSOG / 1200 * Math.Cos(m_fOwnCOG * Math.PI / 180);
            m_dOwnLON += m_fOwnSOG / 1200 * Math.Sin(m_fOwnCOG * Math.PI / 180) ;
            m_dOwnLAT = dOwnLAT;
            //防止角度越界，0-360度
            if (m_fOwnCOG > 360f)
            {
                m_fOwnCOG-=360;
            }
            else if (m_fOwnCOG < 0)
            {
                m_fOwnCOG += 360;
            }
            
            if (m_iOwnHDG > 360f)
            {
                m_iOwnHDG -= 360;
            }
            else if (m_iOwnHDG < 0)
            {
                m_iOwnHDG += 360;
            }
            
            //其他船舶实时计算
            for (int i = 0; i < m_iTotal; i++)
            {
               
                //每过三秒计算一次
                
                float fDistance = 0;
                //定义位移
                fDistance = m_fSOG[i] /1200;             
                double dLAT = 0;
                //实时计算航向
                m_fCOG[i] += m_fROT[i] * fTime_ROT;
                //实时计算船艏向
                m_iHDG[i] = (int)(m_iHDG[i] + m_fROT[i] * fTime_ROT);
                //实时计算经纬度  
                dLAT = m_dLAT[i] + fDistance * Math.Cos(m_fCOG[i]*Math.PI /180 );         
                m_dLON[i] += fDistance * Math.Sin(m_fCOG[i] * Math.PI / 180) ;
                m_dLAT[i] = dLAT;   
                //报警临时参数
                double tmp_RNG = m_dRNG[i];    
                //实时计算与本船距离
                m_dRNG[i] = Math.Sqrt(Math.Pow(Math.Abs(m_dLAT[i] - m_dOwnLAT), 2) + Math.Pow(Math.Abs(m_dLON[i] - m_dOwnLON), 2));
                //船舶侵入丢失报警
                if (tmp_RNG > m_fGuardRNG && m_dRNG[i] < m_fGuardRNG)
                {
                    Warn_Sound();
                }
                else if (tmp_RNG < 50 && m_dRNG[i] > 50)
                {
                    Warn_Sound();
                }
               //实时计算方位（本船为基准）
                if (m_dLAT[i] - m_dOwnLAT == 0)
                { 
                    if(m_dLON[i] >m_dOwnLON)
                    {
                        m_dBRG[i] = 90; 
                    }
                    else if (m_dOwnLON > m_dLON[i])
                    {
                      m_dBRG[i] = 270;
                    }                               
                }
                else if (m_dLON[i] - m_dOwnLON == 0)
                {
                    if (m_dLAT[i] > m_dOwnLAT)
                    {
                        m_dBRG[i] = 0;
                    }
                    else if (m_dOwnLAT > m_dLAT[i])
                    {
                        m_dBRG[i] = 180;
                    }    
                }
               
                
                    else if (m_dOwnLON < m_dLON[i] && m_dOwnLAT < m_dLAT[i])
                    { m_dBRG[i] = Math.Atan(Math.Abs(m_dOwnLON - m_dLON[i]) / Math.Abs(m_dOwnLAT - m_dLAT[i])) * 180 / Math.PI; }
                    else if (m_dOwnLON < m_dLON[i] && m_dOwnLAT > m_dLAT[i])
                    { m_dBRG[i] = Math.Atan(Math.Abs(m_dOwnLAT- m_dLAT[i]) / Math.Abs(m_dOwnLON - m_dLON[i])) * 180 / Math.PI+90; }
                    else if (m_dOwnLON > m_dLON[i] && m_dOwnLAT > m_dLAT[i])
                    { m_dBRG[i] = Math.Atan(Math.Abs(m_dOwnLON - m_dLON[i]) / Math.Abs(m_dOwnLAT - m_dLAT[i])) * 180 / Math.PI + 180; }
                    else if (m_dOwnLON > m_dLON[i] && m_dOwnLAT < m_dLAT[i])
                    { m_dBRG[i] = Math.Atan(Math.Abs(m_dOwnLAT - m_dLAT[i]) / Math.Abs(m_dOwnLON - m_dLON[i])) * 180 / Math.PI + 270; }
              
            
                //防止角度越界，0-360度
                if (m_fCOG[i] > 360f)
                {
                    m_fCOG[i] -= 360;
                }
                else if (m_fCOG[i] < 0)
                {
                    m_fCOG[i] += 360;
                }

                if (m_iHDG[i] > 360f)
                {
                    m_iHDG[i] -= 360;
                }
                else if (m_iHDG[i] < 0)
                {
                    m_iHDG[i] += 360;
                }
                //计算相对航向,为计算CPA，TCPA准备
                //伪相对航向，其他船舶与本船航向直接相减
               float fRel_COG = 0;
               //真相对航向
               double dRel_RealCOG = 0;
               //计算真相对航向的Q参数
               double dQ = 0;
               double dRel_SOG = 0;
                fRel_COG = m_fOwnCOG - m_fCOG[i];
                //利用余弦定理求本船与其他船舶相对船速
                dRel_SOG=Math.Sqrt(Math.Pow(m_fSOG[i],2)+Math.Pow(m_fOwnSOG,2)-2* Math.Cos(fRel_COG*Math.PI /180)*m_fSOG[i]*m_fOwnSOG);
                dQ = Math.Acos((Math.Pow( dRel_SOG,2)+Math.Pow(m_fOwnSOG,2)-Math.Pow(m_fSOG[i],2))/(2*m_fOwnSOG* dRel_SOG))*180/Math.PI;
                //计算真相对航向
                if (fRel_COG > 0)
                {
                     dRel_RealCOG=m_fOwnCOG+dQ;
                }
                else if (fRel_COG < 0)
                {
                   dRel_RealCOG = m_fOwnCOG-dQ;
                }
                //计算CPA
                m_dCPA[i] = (double)(Math.Abs(m_dRNG[i] * Math.Sin((m_dBRG[i] - dRel_RealCOG) * Math.PI / 180)));
                //计算TCPA
                m_fTCPA[i] = (float)((60 * m_dRNG[i] * Math.Cos((m_dBRG[i] - dRel_RealCOG) * Math.PI / 180) / dRel_SOG));
       
            }
         
            for (int i = 0; i < m_iTotal;i++ )
            {
                //其他船舶数据刷新
                m_strData[i, 0] = ((int)m_dBRG[i]).ToString();
                //HEAD UP方位计算
                if (m_iTitle_Choose == 2 || m_iTitle_Choose == 4 || m_iTitle_Choose == 6)
                {
                    int tmp_BRG = (int)(m_dBRG[i] - m_fOwnCOG);
                    if (tmp_BRG < 0)
                    { tmp_BRG += 360; }
                    m_strData[i, 0] = ((double)tmp_BRG).ToString("0.0");                             
                }               
                m_strData[i, 1] = m_dRNG[i].ToString("0.00");
                m_strData[i, 10] = ((int)(m_dLAT[i] / 60)).ToString("00") + "° " + (m_dLAT[i] % 60).ToString("00.0000");
                m_strData[i, 11] = ((int)(m_dLON[i] / 60)).ToString("000") + "° " + (m_dLON[i] % 60).ToString("00.0000");
                m_strData[i, 12] = m_fCOG[i].ToString("0.0");
                m_strData[i, 13] = m_iHDG[i].ToString("0.0");
                m_strData[i, 5] = m_dCPA[i].ToString("0.0");
                m_strData[i, 6] = m_fTCPA[i].ToString("0.0");
            }
         
            Sort_Data();
           

            m_strOwnshipData[6] = ((int)(m_dOwnLAT / 60)).ToString("00") + "°";//本船数据刷新
            m_strOwnshipData[20] = (m_dOwnLAT % 60).ToString("00.0000");
            m_strOwnshipData[7] = ((int)(m_dOwnLON / 60)).ToString("000") + "°";
            m_strOwnshipData[21] = (m_dOwnLON % 60).ToString("00.0000");
            m_strOwnshipData[8] = m_fOwnCOG.ToString("0.0");
           if(m_iFunOrder !=2)
            {
            Data_Display();
            }
            if (m_iFunOrder == 2)
            {
                RealTime_Disp();
            }
         
            
         
           
            


        }
        //船舶旋转和船速显示
        private void Ship_RotateAndSpeed(float SOG,int HDG,OtherShip SHIP)
        {
            //控制船舶旋转
       
                RotateTransform Rtt = new RotateTransform(HDG, 20, 30);
                SHIP.RenderTransform = Rtt;
     
            //船速显示
            if (SOG == 0)
            {
                SHIP.Speed.Height = 0;
            }
            else if (SOG > 0 && SOG < 14)
            {
                SHIP.Speed.Height = 10;
            }
            else if (SOG > 14 && SOG < 23)
            {
                SHIP.Speed.Height = 20;
            }
            else if (SOG > 23)
            {
                SHIP.Speed.Height = 30;
            }
         
            

         
           
            

        }
        //实时图像显示
        private void RealTime_Disp()
        {
            
                //清除所有船舶
                canvas_2_Ship.Children.Clear();
                //初始化本船图像显示       
                OtherShip Mysp = new OtherShip();
                Mysp.Ship.Fill = Brushes.Black;
                //绘制拐向
                if (m_fOwnROT> 0)
                {
                    //根据船速确定位置
                    if (m_fOwnSOG > 0 && m_fOwnSOG< 14)
                    {
                        Mysp.TURN.Margin = new Thickness(15, 15, 0, 0);
                    }
                    else if (m_fOwnSOG > 14 && m_fOwnSOG < 23)
                    {
                        Mysp.TURN.Margin = new Thickness(15, 10, 0, 0);
                    }
                    else if (m_fOwnSOG > 23)
                    {
                        Mysp.TURN.Margin = new Thickness(15, 5, 0, 0);
                    }
                }
                else if (m_fOwnROT < 0)
                {
                    //根据船速确定位置
                    if (m_fOwnSOG > 0 && m_fOwnSOG < 14)
                    {
                        Mysp.TURN.Margin = new Thickness(10, 15, 0, 0);
                    }
                    else if (m_fOwnSOG > 14 && m_fOwnSOG< 23)
                    {
                        Mysp.TURN.Margin = new Thickness(10, 10, 0, 0);
                    }
                    else if (m_fOwnSOG > 23)
                    {
                        Mysp.TURN.Margin = new Thickness(10, 5, 0, 0);
                    }
                }
                else
                {
                    Mysp.TURN.Visibility = Visibility.Collapsed;
                }
                Mysp.Margin = new Thickness(175, 140, 0, 0);
                Ship_RotateAndSpeed(m_fOwnSOG, m_iOwnHDG, Mysp);
                canvas_2_Ship.Children.Add(Mysp);
                //依次建立各船舶
            for (int i = 0; i < m_iTotal; i++)
            {
               
                //船舶小于显示距离（外圈圆半径）才显示
                if (m_dRNG[i] < m_fDispRNG)
                {
                    
                    OtherShip OtSp = new OtherShip();
                    OtSp.Name = "OtrShip" + i.ToString();
                    //绘制被选择船舶外边框
                    if (rectangle_ChoseSign_Top.Visibility == Visibility.Collapsed && i == m_DispChoose)
                    {
                        OtSp.BorderPart1.Visibility = Visibility.Visible;
                        OtSp.BorderPart2.Visibility = Visibility.Visible;
                        OtSp.BorderPart3.Visibility = Visibility.Visible;
                        OtSp.BorderPart4.Visibility = Visibility.Visible;
                        OtSp.BorderPart5.Visibility = Visibility.Visible;
                        OtSp.BorderPart6.Visibility = Visibility.Visible;
                        OtSp.BorderPart7.Visibility = Visibility.Visible;
                        OtSp.BorderPart8.Visibility = Visibility.Visible;

                    }
                    else
                    {
                        OtSp.BorderPart1.Visibility = Visibility.Collapsed;
                        OtSp.BorderPart2.Visibility = Visibility.Collapsed;
                        OtSp.BorderPart3.Visibility = Visibility.Collapsed;
                        OtSp.BorderPart4.Visibility = Visibility.Collapsed;
                        OtSp.BorderPart5.Visibility = Visibility.Collapsed;
                        OtSp.BorderPart6.Visibility = Visibility.Collapsed;
                        OtSp.BorderPart7.Visibility = Visibility.Collapsed;
                        OtSp.BorderPart8.Visibility = Visibility.Collapsed;
                    }
                    //绘制船舶拐向
                    if (m_fROT[i] > 0)
                    {
                        //根据船速确定位置
                        if (m_fSOG[i] > 0 && m_fSOG[i] < 14)           
                        {                           
                            OtSp.TURN.Margin = new Thickness(15,15,0,0);          
                        }
                        else if (m_fSOG[i] > 14 && m_fSOG[i] < 23)         
                        {                                        
                            OtSp.TURN.Margin = new Thickness(15, 10, 0, 0);
                        }
                        else if (m_fSOG[i] > 23)           
                        {                                            
                            OtSp.TURN.Margin = new Thickness(15, 5, 0, 0);
                        }
                    }
                    else if (m_fROT[i] < 0)
                    {
                        //根据船速确定位置
                        if (m_fSOG[i] > 0 && m_fSOG[i] < 14)
                        {
                            OtSp.TURN.Margin = new Thickness(10, 15, 0, 0);
                        }
                        else if (m_fSOG[i] > 14 && m_fSOG[i] < 23)
                        {
                            OtSp.TURN.Margin = new Thickness(10, 10, 0, 0);
                        }
                        else if (m_fSOG[i] > 23)
                        {
                            OtSp.TURN.Margin = new Thickness(10, 5, 0, 0);
                        }
                    }
                    else
                    {
                        OtSp.TURN.Visibility = Visibility.Collapsed;
                    }

                    //以本船为原点，计算其他船舶坐标，本船Margin(175,140,0,0)
                    double tmp_X = 0;
                    double tmp_Y = 0;
                    //根据显示范围，计算相对坐标                   
                        tmp_X = m_dRNG[i] * Math.Sin(m_dBRG[i] / 180 * Math.PI) * 140/m_fDispRNG;
                        tmp_Y = m_dRNG[i] * Math.Cos(m_dBRG[i] / 180 * Math.PI) * 140 / m_fDispRNG;
                   //得到船舶在容器中的位置 
                        OtSp.Margin = new Thickness(tmp_X +175, -tmp_Y + 140, 0, 0);
                   //船舶旋转和速度显示     
                    Ship_RotateAndSpeed(m_fSOG[i],m_iHDG[i],OtSp);
                    //船舶显示
                    canvas_2_Ship.Children.Add(OtSp);
                
                    

                }
            }
                //船首向上显示
            if (m_iBRG_Disp == 1)
            {
                RotateTransform Rtt = new RotateTransform(-m_iOwnHDG, 195, 170);
                canvas_2_Ship.RenderTransform = Rtt;
            }
                //北向上显示
            else
            {
                RotateTransform Rtt = new RotateTransform(0, 195, 170);
                canvas_2_Ship.RenderTransform = Rtt;
            }


          
        }
        //按RNG或TCPA排序
        private void Sort_Data()
        {      
            if (m_iSORT_Disp == 0)
            {

                //使用冒泡法，以RNG为基准，将其他船舶信息排序
                for (int i = 0; i < m_iTotal -1; i++)
                    for (int j = 0; j < m_iTotal-1; j++)
                    {
                        double tmp_dRNG = 0;
                        double tmp_dLAT = 0;
                        double tmp_dLON = 0;
                        float tmp_fCOG = 0;
                        float tmp_fROT = 0;
                        float tmp_fSOG = 0;
                        int tmp_iHDG = 0;
                        double tmp_dBRG = 0;
                        float tmp_fTCPA = 0;
                    
                        string[] tmp_strData = new string[24];
                        if (m_dRNG[j] > m_dRNG[j + 1])
                        {                                                                    
                            //RNG数组排序
                            tmp_dRNG = m_dRNG[j];
                            m_dRNG[j] = m_dRNG[j + 1];
                            m_dRNG[j + 1] = tmp_dRNG;
                            //其他参与计算和显示的数组都以RNG为基准
                            //ROT排序
                            tmp_fROT = m_fROT[j];
                            m_fROT[j] = m_fROT[j + 1];
                            m_fROT[j + 1] = tmp_fROT;
                            //HDG排序
                            tmp_iHDG = m_iHDG[j];
                            m_iHDG[j] = m_iHDG[j+1];
                            m_iHDG[j + 1] = tmp_iHDG;
                            //LAT排序
                            tmp_dLAT = m_dLAT[j];
                            m_dLAT[j] = m_dLAT[j + 1];
                            m_dLAT[j + 1] = tmp_dLAT;
                            //LON排序
                            tmp_dLON = m_dLON[j];
                            m_dLON[j] = m_dLON[j + 1];
                            m_dLON[j + 1] = tmp_dLON;
                            //COG排序
                            tmp_fCOG = m_fCOG[j];
                            m_fCOG[j] = m_fCOG[j + 1];
                            m_fCOG[j + 1] = tmp_fCOG;
                            //SOG排序
                            tmp_fSOG = m_fSOG[j];
                            m_fSOG[j] = m_fSOG[j + 1];
                            m_fSOG[j + 1] = tmp_fSOG;
                            //BRG排序
                            tmp_dBRG = m_dBRG[j];
                            m_dBRG[j] = m_dBRG[j + 1];
                            m_dBRG[j + 1] = tmp_dBRG;
                            //
                            tmp_fTCPA = m_fTCPA[j];
                            m_fTCPA[j] = m_fTCPA[j + 1];
                            m_fTCPA[j + 1] = tmp_fTCPA;


                            //strData数组排序
                         
                            for (int k = 0; k < 24; k++)
                            {
                                tmp_strData[k] = m_strData[j, k];
                            }
                            for (int k = 0; k < 24; k++)
                            {
                                m_strData[j, k] = m_strData[j + 1, k];
                            }
                            for (int k = 0; k < 24; k++)
                            {
                                m_strData[j + 1, k] = tmp_strData[k];
                            }
                        
                      
                            
                            
                        }
                    }
            }
            //使用冒泡法，以TCPA为基准，将其他船舶信息排序
            else if (m_iSORT_Disp == 1)
            {
              
              
                for (int i = 0; i < m_iTotal-1; i++)
                    for (int j = 0; j < m_iTotal-1; j++)
                    {
                      
                        if (Math.Abs(m_fTCPA[j] )> Math.Abs(m_fTCPA[j + 1]))
                        {
                            float tmp_fTCPA = 0;
                            double tmp_dRNG = 0;
                            double tmp_dLAT = 0;
                            double tmp_dLON = 0;
                           float tmp_fROT = 0;
                            float tmp_fCOG = 0;
                            float tmp_fSOG = 0;                           
                            double tmp_dBRG = 0;
                            int tmp_iHDG = 0;

                           
                        
                            string[] tmp_strData1= new string[24];
                            //以TCPA为基准排序
                            tmp_fTCPA = m_fTCPA[j];
                            m_fTCPA[j] = m_fTCPA[j + 1];
                            m_fTCPA[j + 1] = tmp_fTCPA;
                            //其他参与计算和显示的数组都以TCPA为基准
                            //RNG排序
                            tmp_dRNG = m_dRNG[j];
                            m_dRNG[j] = m_dRNG[j + 1];
                            m_dRNG[j + 1] = tmp_dRNG;
                            //ROT排序
                            tmp_fROT = m_fROT[j];
                            m_fROT[j] = m_fROT[j + 1];
                            m_fROT[j + 1] = tmp_fROT;
                            //HDG排序
                            tmp_iHDG = m_iHDG[j];
                            m_iHDG[j] = m_iHDG[j + 1];
                            m_iHDG[j + 1] = tmp_iHDG;
                            //LAT排序
                            tmp_dLAT = m_dLAT[j];
                            m_dLAT[j] = m_dLAT[j + 1];
                            m_dLAT[j + 1] = tmp_dLAT;
                            //LON排序
                            tmp_dLON = m_dLON[j];
                            m_dLON[j] = m_dLON[j + 1];
                            m_dLON[j + 1] = tmp_dLON;
                            //COG排序
                            tmp_fCOG = m_fCOG[j];
                            m_fCOG[j] = m_fCOG[j + 1];
                            m_fCOG[j + 1] = tmp_fCOG;
                            //SOG排序
                            tmp_fSOG = m_fSOG[j];
                            m_fSOG[j] = m_fSOG[j + 1];
                            m_fSOG[j + 1] = tmp_fSOG;
                            //BRG排序
                            tmp_dBRG = m_dBRG[j];
                            m_dBRG[j] = m_dBRG[j + 1];
                            m_dBRG[j + 1] = tmp_dBRG;
                            //strData数组排序

                            for (int k = 0; k < 24; k++)
                            {
                                tmp_strData1[k] = m_strData[j, k];
                            }
                            for (int k = 0; k < 24; k++)
                            {
                                m_strData[j, k] = m_strData[j + 1, k];
                            }
                            for (int k = 0; k < 24; k++)
                            {
                                m_strData[j + 1, k] = tmp_strData1[k];
                            }
                        }
                    }
            }
        }
        //显示当前时间功能，定时器函数
        private void Cal_Time()
        {

            m_Timer.Stop();
            //开启定时器
            m_Timer.Start();
           
        }
        //显示当前时间功能，定时器事件
        private void Time_Display(object sender, EventArgs e)
        {
            //根据时区显示本地时间
            if (m_bUTC == false)
            {
                if (m_dOwnLON > -450 && m_dOwnLON < 450)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-8d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 450 && m_dOwnLON < 1350)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-7d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 1350 && m_dOwnLON < 2250)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-6d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 2250 && m_dOwnLON < 3150)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-5d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 3150 && m_dOwnLON < 4050)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-4d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 4050 && m_dOwnLON < 4950)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-3d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 4950 && m_dOwnLON < 5850)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-2d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 5850 && m_dOwnLON < 6750)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-1d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 6750 && m_dOwnLON < 7650)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.ToString("HH:mm");
                }
                else if (m_dOwnLON > 7650 && m_dOwnLON < 8550)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(1d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 8550 && m_dOwnLON < 9450)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(2d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 9450 && m_dOwnLON < 10350)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(3d).ToString("HH:mm");
                }
                else if (m_dOwnLON > 10350)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(4d).ToString("HH:mm");
                }
                else if (m_dOwnLON < -10350)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-20d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -10350 && m_dOwnLON < -9450)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-19d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -9450 && m_dOwnLON < -8550)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-18d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -8550 && m_dOwnLON < -7650)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-17d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -7650 && m_dOwnLON < -6750)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-16d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -6750 && m_dOwnLON < -5850)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-15d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -5850 && m_dOwnLON < -4950)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-14d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -4950 && m_dOwnLON < -4050)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-13d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -4050 && m_dOwnLON < -3150)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-12d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -3150 && m_dOwnLON < -2250)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-11d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -2250 && m_dOwnLON < -1350)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-10d).ToString("HH:mm");
                }
                else if (m_dOwnLON > -1350 && m_dOwnLON < -450)
                {
                    textBlock_Time.Text = "LMT" + DateTime.Now.AddHours(-9d).ToString("HH:mm");
                }
            }
            else
            {
                textBlock_Time.Text = "UTC" + DateTime.Now.AddHours(-8d).ToString("HH:mm");
            }
            textBlock1.Text = "m_iFunorder=" + m_iFunOrder.ToString();

            textBlock3.Text = "mm=" + m_iWpChoose.ToString();
            
        }
        //开机功能（按下PWR键一秒后开机），定时器函数
        private void Cal_Time_Powon()
        {

            
            //开启定时器
            m_PowOn.Start();

        }
        //开机功能（按下PWR键一秒后开机），定时器事件
        private void Power_On(object sender, EventArgs e)
        {
            if (m_iFunOrder == 0&&image4.Visibility==Visibility.Visible )
            {
                //给逻辑变量赋值，显示主界面
                m_iFunOrder = 1;
                //绘制界面、高亮，显示数据，开启实时计算定时器
                Draw_My_Screen(m_Struct_No1);
                Draw_Choose(0);
                RealTime_Cal(null, null);
                Data_Display();            
                m_RealTimeCal.Start();
                image_ON.Visibility = Visibility.Visible;
            }
            else if (m_iFunOrder != 0 && m_bPower == true && image4_ON.Visibility == Visibility.Visible && image5_ON.Visibility == Visibility.Visible)
            {
                image_ON.Visibility = Visibility.Collapsed;
                m_iFunOrder = 0;
                Draw_Choose(0);
                m_RealTimeCal.Stop();
                Initial_My_Struct();
                m_strPassCodeinPut = "";            
                Draw_My_Button(0, 4);
                m_PowOn.Stop();
                m_bPower = false;
                m_bIsShowDoubleHand = false;
                Grid_Main.Cursor = Cursors.Hand;
                image4_ON.Visibility = Visibility.Collapsed;
                image5_ON.Visibility = Visibility.Collapsed;
                image_HandOFF.Visibility = Visibility.Collapsed;
                image_HandPow.Visibility = Visibility.Collapsed;

            }
       

           
        }
        //快速编辑功能，定时器事件
        private void Quick_Edit(object sender, EventArgs e)
        {
            //快速增加
            if (m_iQuickEdit == 1)
            {
                if (m_iRotateDown == 1)
                {
                    //绘制旋钮

                    Count_Rotate++;
                    if (Count_Rotate == 20)
                    {
                        Count_Rotate = 0;
                    }

                    RG_123.Rect = new Rect(Count_Rotate * 157, 0, 157, 157);
                    image11.Margin = new Thickness(43 - Count_Rotate * 157, 5, 0, 0);
                }
                
                if (m_iFunOrder == 60 && m_iMainMenu_60 == 7)
                {
                    m_fOwnDraught += 0.1f;
                    if (m_fOwnDraught > 25)
                    {
                        m_fOwnDraught = 0;
                    }
                    m_strOwnshipData[16] = m_fOwnDraught.ToString("0.0");
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = m_strOwnshipData[16] + "M";
                }
                else if (m_iFunOrder == 60 && m_iMainMenu_60 == 15)
                {
                    m_iPersons++;
                    if (m_iPersons >50)
                    {
                        m_iPersons = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = m_iPersons.ToString();
                }
                else if (m_iFunOrder == 60 && m_iMainMenu_60 == 17)
                {
                    m_iHeightKeel += 0.1f;
                    if (m_iHeightKeel > 100)
                    {
                        m_iHeightKeel = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = m_iHeightKeel.ToString("0.0") + "M";
                }
                else if (m_iFunOrder == 62 && m_iMainMenu_62 == 2)
                {
                    m_fGuardRNG += 0.1f;
                    if (m_fGuardRNG > 99)
                    {
                        m_fGuardRNG = 0;
                    }
                    Alarm_Setup();
                }
                else if (m_iFunOrder == 62 && m_iMainMenu_62 == 5)
                {
                    m_fLostRNG += 0.1f;
                    if (m_fLostRNG > 99)
                    {
                        m_fLostRNG = m_fGuardRNG + 0.1f;
                    }
                    Alarm_Setup();
                }
                
            }
                //快速减小
            else if (m_iQuickEdit == -1)
            {
                if (m_iRotateDown == -1)
                {
                    //绘制旋钮

                    Count_Rotate--;
                    if (Count_Rotate == -1)
                    {
                        Count_Rotate = 19;
                    }

                    RG_123.Rect = new Rect(Count_Rotate * 157, 0, 157, 157);
                    image11.Margin = new Thickness(43 - Count_Rotate * 157, 5, 0, 0);
                }
                if (m_iFunOrder == 60 && m_iMainMenu_60 == 7)
                {
                    m_fOwnDraught -= 0.1f;
                    if (m_fOwnDraught < 0)
                    {
                        m_fOwnDraught = 25;
                    }
                    m_strOwnshipData[16] = m_fOwnDraught.ToString("0.0");
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = m_strOwnshipData[16] + "M";
                }
                else if (m_iFunOrder == 60 && m_iMainMenu_60 == 15)
                {
                    m_iPersons--;
                    if (m_iPersons < 0)
                    {
                        m_iPersons = 50;
                    }
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = m_iPersons.ToString();
                }
                else if (m_iFunOrder == 60 && m_iMainMenu_60 == 17)
                {
                    m_iHeightKeel -= 0.1f;
                    if (m_iHeightKeel < 0)
                    {
                        m_iHeightKeel = 100;
                    }
                    (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = m_iHeightKeel.ToString("0.0") + "M";
                }
                else if (m_iFunOrder == 62 && m_iMainMenu_62 == 2)
                {
                    m_fGuardRNG -= 0.1f;
                    if (m_fGuardRNG < 0)
                    {
                        m_fGuardRNG = 99;
                    }
                    Alarm_Setup();
                }
                else if (m_iFunOrder == 62 && m_iMainMenu_62 == 5)
                {
                    m_fLostRNG -= 0.1f;
                    if (m_fLostRNG > m_fGuardRNG + 1)
                    {
                        m_fLostRNG -= 0.1f;
                    }

                    if (m_fLostRNG < 0)
                    {
                        m_fLostRNG = 99;
                    }
                    Alarm_Setup();
                }
            }
        }
        //生成不同界面框架及控件，用m_iFunOrder区分
        private void Draw_My_Screen(MY_STRUCT Stu)
        {
            //主界面
            if (m_iFunOrder == 1)
            {
                Grid_1.Visibility = Visibility.Visible;
                Grid_2.Visibility = Visibility.Collapsed;
                Grid_2_3_5_6.Visibility = Visibility.Collapsed;
                Grid_4.Visibility = Visibility.Collapsed;
                Grid_5.Visibility = Visibility.Collapsed;
                Grid_6.Visibility = Visibility.Collapsed;
                Grid_DetailSel.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Up_Copy.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                rectangle_Title.Visibility = Visibility.Visible;
                rectangle_MenuTitleLeft.Visibility = Visibility.Visible;
                rectangle_MenuTitleRight.Visibility = Visibility.Visible;
              
                rectangle_Total.Visibility = Visibility.Visible;
                rectangle_Bottom.Visibility = Visibility.Visible;
                
                textBlock_Title.Visibility = Visibility.Visible;
                textBlock_MenuTitleLeft.Visibility = Visibility.Visible;
                textBlock_MenuTitleRight.Visibility = Visibility.Visible;
                textBlock_MenuTitleMid.Visibility = Visibility.Visible;
                textBlock_Time.Visibility = Visibility.Visible;
                textBlock_Total.Visibility = Visibility.Visible;
                textBlock_Bottom.Visibility = Visibility.Visible;
                Grid_InterRogation.Visibility = Visibility.Collapsed;
                //控制滚动标记显示（上下三角）
                if (m_Count_Choose_Up != 0)
                {
                    polygon_ChoseSign_Up.Visibility = Visibility.Visible;
                   
                }
                 if (m_Count_Choose_Up != 4)
                {
                    polygon_ChoseSign_Down.Visibility = Visibility.Visible;          
                }

                Grid_3.Visibility = Visibility.Collapsed;
                image_ON.Visibility = Visibility.Visible;
                
                Cal_Time();
            }
            //DSPL界面
            if (m_iFunOrder == 2)
            {
                Grid_1.Visibility = Visibility.Collapsed;
                Grid_2.Visibility = Visibility.Visible;
                Grid_2_3_5_6.Visibility = Visibility.Visible;
                Grid_3.Visibility = Visibility.Collapsed;
                Grid_4.Visibility = Visibility.Collapsed;
                Grid_5.Visibility = Visibility.Collapsed;
                Grid_TX.Visibility = Visibility.Collapsed;
                Grid_TX_Send.Visibility = Visibility.Collapsed;
                Grid_6.Visibility = Visibility.Collapsed;
                Grid_DetailSel.Visibility = Visibility.Collapsed;
                Grid_OtrSel.Visibility = Visibility.Collapsed;
                Grid_Input.Visibility = Visibility.Collapsed;
                Grid_WayPoint.Visibility = Visibility.Collapsed;
                Grid_ETA.Visibility = Visibility.Collapsed;
                Grid_WayPointEdit.Visibility = Visibility.Collapsed;
                Grid_InterRogation.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Up_Copy.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                rec_2_Down.Visibility = Visibility.Collapsed;
                warpPanel_2_Setup.Visibility = Visibility.Collapsed;
                stackPanel_2_Stakpnl_Setup.Visibility = Visibility.Visible;
                rectangle_2_Stakpnl_Setup.Visibility = Visibility.Visible;
                canvas_2_Map.Visibility = Visibility.Visible;
                canvas_2_Ship.Visibility = Visibility.Visible;
                (stackPanel_2_Stakpnl_Setup.Children[1] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_2_Stakpnl_Setup.Children[1] as TextBlock).Background = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));;




               
                rectangle_Total.Visibility = Visibility.Collapsed;
             
                textBlock_Total.Visibility = Visibility.Collapsed;
               
                Grid_3.Visibility = Visibility.Collapsed;
                (stackPanel_2_Stakpnl_Setup.Children[0] as TextBlock).Text = m_fDispRNG.ToString()+"NM";
                (stackPanel_2_Stakpnl_Setup.Children[1] as TextBlock).Text = "[SETUP]";
                polygon_ChoseSign_Up.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                //setup菜单初始化
                (warpPanel_2_Setup.Children[0] as TextBlock).Text = "1.RANGE";
                (warpPanel_2_Setup.Children[1] as TextBlock).Text = m_fDispRNG.ToString()+"NM";
                (warpPanel_2_Setup.Children[2] as TextBlock).Text = "2.BEARING";
                if (m_iBRG_Disp == 0)
                {
                    (warpPanel_2_Setup.Children[3] as TextBlock).Text = "NORTH";
                }
                else
                {
                    (warpPanel_2_Setup.Children[3] as TextBlock).Text = "HEAD";
                }
                (warpPanel_2_Setup.Children[4] as TextBlock).Text = "3.SORT";
                if (m_iSORT_Disp == 0)
                {
                    (warpPanel_2_Setup.Children[5] as TextBlock).Text = "RANGE";
                }
                 else if (m_iSORT_Disp == 1)
                {
                    (warpPanel_2_Setup.Children[5] as TextBlock).Text = "TCPA";
                }
                else if (m_iSORT_Disp == 2)
                {
                    (warpPanel_2_Setup.Children[5] as TextBlock).Text = "GROUP";
                }
                (warpPanel_2_Setup.Children[6] as TextBlock).Text = "4.GUARD ZONE";
                (warpPanel_2_Setup.Children[7] as TextBlock).Text = m_fGuardRNG.ToString()+"NM";
                (warpPanel_2_Setup.Children[8] as TextBlock).Text = "5.NUMBER OF SHIPS";
                (warpPanel_2_Setup.Children[9] as TextBlock).Text = m_iNumOfShip.ToString();
                (warpPanel_2_Setup.Children[10] as TextBlock).Text = "6.CONTRAST";
                (warpPanel_2_Setup.Children[11] as TextBlock).Text = m_iContrast.ToString();
                (warpPanel_2_Setup.Children[12] as TextBlock).Text = "7.AUTO RANGE SET";
                if (m_bAutoRNG == false)
                {
                    (warpPanel_2_Setup.Children[13] as TextBlock).Text = "OFF";
                }
                else
                {
                    (warpPanel_2_Setup.Children[13] as TextBlock).Text = "ON";
                }
                (warpPanel_2_Setup.Children[14] as TextBlock).Text = "[EXIT]";
                (warpPanel_2_Setup.Children[16] as TextBlock).Text = "[ENT]";



                




         
    
            }
            //其他船舶或本船，详细数据界面
            if (m_iFunOrder == 3||m_iFunOrder==43)
            {
                Grid_1.Visibility = Visibility.Collapsed;
                Grid_2.Visibility = Visibility.Collapsed;
                Grid_2_3_5_6.Visibility = Visibility.Visible;
                Grid_4.Visibility = Visibility.Collapsed;
               
                rectangle_Total.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
              
                textBlock_Total.Visibility = Visibility.Collapsed;
                rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                
               Grid_3.Visibility = Visibility.Visible;
                polygon_ChoseSign_Up.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down.Visibility = Visibility.Collapsed;
            }
            //设置界面
            if (m_iFunOrder == 4)
            {
                Grid_1.Visibility = Visibility.Collapsed;
                Grid_4.Visibility = Visibility.Visible;
                Grid_3.Visibility = Visibility.Collapsed;
                Grid_2_3_5_6.Visibility = Visibility.Collapsed;
                grid_4_Down.Visibility = Visibility.Visible;
                Grid_DetailSel.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                grid_4_Down_Ownship.Visibility = Visibility.Collapsed;
                rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Up.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down.Visibility = Visibility.Collapsed;
                rectangle_Total.Visibility = Visibility.Collapsed;
                textBlock_Total.Visibility = Visibility.Collapsed;
                warpPanel_4_Down.Visibility = Visibility.Visible;
                warpPanel_4_Down_Owndisp.Visibility = Visibility.Collapsed;
                polygon_4_ChoseSign_Up.Visibility = Visibility.Collapsed;
                (warpPanel_4_Down.Children[0] as TextBlock).Text = "[EXIT]";
                (warpPanel_4_Down.Children[1] as TextBlock).Text = "[LIST]";
                (warpPanel_4_Down.Children[2] as TextBlock).Text = "[OWN POS DISP]";
                (warpPanel_4_Down.Children[3] as TextBlock).Text = "[OWN DETAIL]";
                (warpPanel_4_Down.Children[4] as TextBlock).Text = "[PGDN]";
                (warpPanel_4_Down.Children[5] as TextBlock).Text = "[PGUP]";
                textBlock_4_Owndisp.Text = "OWN POS DISP";
                grid_4_Down_List.Visibility = Visibility.Collapsed;
                (warpPanel_4_Down_Owndisp.Children[0] as TextBlock).Text = "ON";
                (warpPanel_4_Down_Owndisp.Children[1] as TextBlock).Text =  "OFF";
                (warpPanel_4_Down_List.Children[0] as TextBlock).Text = "BRG ";
                (warpPanel_4_Down_List.Children[1] as TextBlock).Text = "SORT";
                (warpPanel_4_Down_List.Children[2] as TextBlock).Text = "NAME";
                (warpPanel_4_Down_List_Sybl.Children[0] as TextBlock).Text = ":";
                (warpPanel_4_Down_List_Sybl.Children[1] as TextBlock).Text = ":";
                (warpPanel_4_Down_List_Sybl.Children[2] as TextBlock).Text = ":";
                Tb_Sybl.Text = "/";
                Tb_Sybl2.Text = "/";
                Tb_Sybl3.Text = "/";
                Tb_Sybl4.Text = "/";
                   
                (warpPanel_4_Down_BRG.Children[0] as TextBlock).Text = " HEAD UP ";
                (warpPanel_4_Down_BRG.Children[1] as TextBlock).Text = "NORTH UP";
                (warpPanel_4_Down_SORT.Children[0] as TextBlock).Text = " RANGE";
                (warpPanel_4_Down_SORT.Children[1] as TextBlock).Text = " TCPA ";
                (warpPanel_4_Down_SORT.Children[2] as TextBlock).Text = "GROUP";
                (warpPanel_4_Down_NAME.Children[0] as TextBlock).Text = " SHIP NAME";
                (warpPanel_4_Down_NAME.Children[1] as TextBlock).Text = " MMSI";
              

            }
            //输入（虚拟键盘）界面
            if (m_iFunOrder == 5)
            {
                Grid_1.Visibility = Visibility.Collapsed;
                Grid_5.Visibility = Visibility.Visible;
                Grid_2_3_5_6.Visibility = Visibility.Visible;
                rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                textBlock_Total.Visibility = Visibility.Collapsed;
                rectangle_Total.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down.Visibility = Visibility.Collapsed;
                for(int i=0;i<16;i++)
                {
                    (warpPanel_5_Input.Children[i] as TextBlock).Text = ((char)(65 + i)).ToString() ;
                 }
                (warpPanel_5_Input.Children[16] as TextBlock).Text = "↑";
                for (int i = 17; i < 27; i++)
                {
                    (warpPanel_5_Input.Children[i] as TextBlock).Text = ((char)(65 + i-1)).ToString();
                }
                (warpPanel_5_Input.Children[27] as TextBlock).Text = ".";
                (warpPanel_5_Input.Children[28] as TextBlock).Text = " ";
                for (int i = 29; i <33 ; i++)
                {
                    (warpPanel_5_Input.Children[i] as TextBlock).Text = ((char)(19 + i )).ToString();
                }
                (warpPanel_5_Input.Children[33] as TextBlock).Text = "";//下一曲符号
                for (int i = 34; i < 40; i++)
                {
                    (warpPanel_5_Input.Children[i] as TextBlock).Text = ((char)(18+ i)).ToString();
                }
                (warpPanel_5_Input.Children[40] as TextBlock).Text = "[";
                (warpPanel_5_Input.Children[41] as TextBlock).Text = "\\";
                (warpPanel_5_Input.Children[42] as TextBlock).Text = "]";
                (warpPanel_5_Input.Children[43] as TextBlock).Text = "_";
              
                for (int i = 44; i <52 ; i++)
                {
                    (warpPanel_5_Input.Children[i] as TextBlock).Text = ((char)( i-10)).ToString();
                }
                (warpPanel_5_Input.Children[52] as TextBlock).Text = "?";
                (warpPanel_5_Input.Children[53] as TextBlock).Text = "@";
                (warpPanel_5_Input.Children[54] as TextBlock).Text = "+";
                (warpPanel_5_Input.Children[55] as TextBlock).Text = "-";
                (warpPanel_5_Input.Children[56] as TextBlock).Text = "*";
                (warpPanel_5_Input.Children[57] as TextBlock).Text = "/";
                (warpPanel_5_Input.Children[58] as TextBlock).Text = "^";
                (warpPanel_5_Input.Children[59] as TextBlock).Text = ",";
                (warpPanel_5_Input.Children[60] as TextBlock).Text = ":";
                (warpPanel_5_Input.Children[61] as TextBlock).Text = ";";
                (warpPanel_5_Input.Children[62] as TextBlock).Text = "<";
                (warpPanel_5_Input.Children[63] as TextBlock).Text = "=";
                (warpPanel_5_Input.Children[64] as TextBlock).Text = ">";
                (warpPanel_5_Input.Children[65] as TextBlock).Text = "!";

                (stackPanel_5_Setup.Children[0] as TextBlock).Text = "[EXIT]";
                (stackPanel_5_Setup.Children[1] as TextBlock).Text = "[ENT]";
                textBlock_5_Password.Text = "PASSWORD :";
                for (int i = 0; i < 4; i++)
                {
                    (warpPanel_5_Passcode.Children[i] as TextBlock).Text = "*";
                }

            }
            //主菜单界面
            if (m_iFunOrder == 6)
            {
                Grid_1.Visibility = Visibility.Collapsed;
                Grid_2.Visibility = Visibility.Collapsed;
                Grid_2_3_5_6.Visibility = Visibility.Visible;
                Grid_3.Visibility = Visibility.Collapsed;
                Grid_4.Visibility = Visibility.Collapsed;
                Grid_5.Visibility = Visibility.Collapsed;
                Grid_6.Visibility = Visibility.Visible;
                Grid_OtrSel.Visibility = Visibility.Collapsed;
                Grid_Input.Visibility = Visibility.Collapsed;
                Grid_TX.Visibility = Visibility.Collapsed;
                Grid_TX_Send.Visibility = Visibility.Collapsed;
                Grid_DetailSel.Visibility = Visibility.Collapsed;
                Grid_WayPoint.Visibility = Visibility.Collapsed;
                Grid_InterRogation.Visibility = Visibility.Collapsed;
                Grid_ETA.Visibility = Visibility.Collapsed;
                Grid_WayPointEdit.Visibility = Visibility.Collapsed;
                rectangle_Total.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Up_Copy.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                m_iMainMenu_Sel = 0;
                m_iMainMenu_60 = 0;
                m_iMainMenu_61 = 1;
                m_iMainMenu_62 = 1;
                m_iMainMenu_63 = 0;
                m_iMainMenu_64 = 1;
                textBlock_Total.Visibility = Visibility.Collapsed;
                rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Up.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down.Visibility = Visibility.Collapsed;
                (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = " 1.VOYAGE STATIC DATA";
                (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = " 2.MESSAGE";
                (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = " 3.ALARM SETTING";
                (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = " 4.SET UP";
                (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.MAINTENANCE";
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = " -------------------------";
                (stackPanel_6_Mainmenu.Children[10] as TextBlock).Text = " POWER REDUCTION:NORMAL";


                
            }
            if (m_iFunOrder == 7)
            {
                Grid_1.Visibility = Visibility.Collapsed;
                Grid_5.Visibility = Visibility.Collapsed;
                Grid_2_3_5_6.Visibility = Visibility.Visible;
                Grid_OtrSel.Visibility = Visibility.Collapsed;
                Grid_6.Visibility = Visibility.Collapsed;
                Grid_Input.Visibility = Visibility.Visible;
                Grid_TX.Visibility = Visibility.Collapsed;
                rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Up_Copy.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                textBlock_Total.Visibility = Visibility.Collapsed;
                rectangle_Total.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down.Visibility = Visibility.Collapsed;
                
                for (int i = 0; i < 16; i++)
                {
                    (warpPanel_Input.Children[i] as TextBlock).Text = ((char)(65 + i)).ToString();
                }
                (warpPanel_Input.Children[16] as TextBlock).Text = "↑";
                for (int i = 17; i < 27; i++)
                {
                    (warpPanel_Input.Children[i] as TextBlock).Text = ((char)(65 + i - 1)).ToString();
                }
                (warpPanel_Input.Children[27] as TextBlock).Text = ".";
                (warpPanel_Input.Children[28] as TextBlock).Text = " ";
                for (int i = 29; i < 33; i++)
                {
                    (warpPanel_Input.Children[i] as TextBlock).Text = ((char)(19 + i)).ToString();
                }
                (warpPanel_Input.Children[33] as TextBlock).Text = "";//下一曲符号
                for (int i = 34; i < 40; i++)
                {
                    (warpPanel_Input.Children[i] as TextBlock).Text = ((char)(18 + i)).ToString();
                }
                (warpPanel_Input.Children[40] as TextBlock).Text = "[";
                (warpPanel_Input.Children[41] as TextBlock).Text = "\\";
                (warpPanel_Input.Children[42] as TextBlock).Text = "]";
                (warpPanel_Input.Children[43] as TextBlock).Text = "_";

                for (int i = 44; i < 52; i++)
                {
                    (warpPanel_Input.Children[i] as TextBlock).Text = ((char)(i - 10)).ToString();
                }
                (warpPanel_Input.Children[52] as TextBlock).Text = "?";
                (warpPanel_Input.Children[53] as TextBlock).Text = "@";
                (warpPanel_Input.Children[54] as TextBlock).Text = "+";
                (warpPanel_Input.Children[55] as TextBlock).Text = "-";
                (warpPanel_Input.Children[56] as TextBlock).Text = "*";
                (warpPanel_Input.Children[57] as TextBlock).Text = "/";
                (warpPanel_Input.Children[58] as TextBlock).Text = "^";
                (warpPanel_Input.Children[59] as TextBlock).Text = ",";
                (warpPanel_Input.Children[60] as TextBlock).Text = ":";
                (warpPanel_Input.Children[61] as TextBlock).Text = ";";
                (warpPanel_Input.Children[62] as TextBlock).Text = "<";
                (warpPanel_Input.Children[63] as TextBlock).Text = "=";
                (warpPanel_Input.Children[64] as TextBlock).Text = ">";
                (warpPanel_Input.Children[65] as TextBlock).Text = "!";

                (stackPanel_Input_Setup.Children[0] as TextBlock).Text = "[EXIT]";
                (stackPanel_Input_Setup.Children[1] as TextBlock).Text = "[ENT]";
                (stackPanel_Input_Setup.Children[3] as TextBlock).Text = "[CLEAR]";
                textBlock_InputText.Text = m_strOwnshipData[11];
            

            }
            if (m_iFunOrder == 70)
            {
                Grid_1.Visibility = Visibility.Collapsed;
                Grid_5.Visibility = Visibility.Collapsed;
                Grid_2_3_5_6.Visibility = Visibility.Visible;
                Grid_OtrSel.Visibility = Visibility.Collapsed;
                Grid_6.Visibility = Visibility.Collapsed;
                Grid_Input.Visibility = Visibility.Visible;
                Grid_TX.Visibility = Visibility.Collapsed;
                rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Up_Copy.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                textBlock_Total.Visibility = Visibility.Collapsed;
                rectangle_Total.Visibility = Visibility.Collapsed;
                polygon_ChoseSign_Down.Visibility = Visibility.Collapsed;
                for (int i = 0; i < 16; i++)
                {
                    (warpPanel_Input.Children[i] as TextBlock).Text = ((char)(65 + i)).ToString();
                }
                (warpPanel_Input.Children[16] as TextBlock).Text = "↑";
                for (int i = 17; i < 27; i++)
                {
                    (warpPanel_Input.Children[i] as TextBlock).Text = ((char)(65 + i - 1)).ToString();
                }
                (warpPanel_Input.Children[27] as TextBlock).Text = ".";
                (warpPanel_Input.Children[28] as TextBlock).Text = " ";
                for (int i = 29; i < 33; i++)
                {
                    (warpPanel_Input.Children[i] as TextBlock).Text = ((char)(19 + i)).ToString();
                }
                (warpPanel_Input.Children[33] as TextBlock).Text = "";//下一曲符号
                for (int i = 34; i < 40; i++)
                {
                    (warpPanel_Input.Children[i] as TextBlock).Text = ((char)(18 + i)).ToString();
                }
                (warpPanel_Input.Children[40] as TextBlock).Text = "[";
                (warpPanel_Input.Children[41] as TextBlock).Text = "\\";
                (warpPanel_Input.Children[42] as TextBlock).Text = "]";
                (warpPanel_Input.Children[43] as TextBlock).Text = "_";

                for (int i = 44; i < 52; i++)
                {
                    (warpPanel_Input.Children[i] as TextBlock).Text = ((char)(i - 10)).ToString();
                }
                (warpPanel_Input.Children[52] as TextBlock).Text = "?";
                (warpPanel_Input.Children[53] as TextBlock).Text = "@";
                (warpPanel_Input.Children[54] as TextBlock).Text = "+";
                (warpPanel_Input.Children[55] as TextBlock).Text = "-";
                (warpPanel_Input.Children[56] as TextBlock).Text = "*";
                (warpPanel_Input.Children[57] as TextBlock).Text = "/";
                (warpPanel_Input.Children[58] as TextBlock).Text = "^";
                (warpPanel_Input.Children[59] as TextBlock).Text = ",";
                (warpPanel_Input.Children[60] as TextBlock).Text = ":";
                (warpPanel_Input.Children[61] as TextBlock).Text = ";";
                (warpPanel_Input.Children[62] as TextBlock).Text = "<";
                (warpPanel_Input.Children[63] as TextBlock).Text = "=";
                (warpPanel_Input.Children[64] as TextBlock).Text = ">";
                (warpPanel_Input.Children[65] as TextBlock).Text = "!";

                (stackPanel_Input_Setup.Children[0] as TextBlock).Text = "[EXIT]";
                (stackPanel_Input_Setup.Children[1] as TextBlock).Text = "[TX]";
                textBlock_InputText.Text = "";


            }
           
            textBlock_MenuTitleLeft.Text = Stu.Menu_Title_Left;
            textBlock_MenuTitleRight.Text = Stu.Menu_Title_Right;
            textBlock_MenuTitleMid.Text = Stu.Menu_Title_Mid;
         
        }
        //定义高亮显示位置逻辑值，同时用做确定键逻辑值
        int m_Count_Choose = 0;
        //定义滚动高亮显示位置逻辑值,同时用做返回键逻辑值
        int m_Count_Choose_Down = 15;
        int m_Count_Choose_Up = 0;
        //定义船舶详细信息页号逻辑值
        int m_Detail_Choose=0;
        //定义List设置逻辑值
        int m_iList_Select = 0;
        //高亮绘制函数
        private void Draw_Choose(int choose)
        {

            switch (choose)
            {
                //初始化高亮状态
                case 0:
                    {
                        rectangle_ChoseSign_Top.Visibility = Visibility.Visible;
                        TitleDisplay(7);
                        BottomDisplay();
                        if (m_bOwnposdisp == false)
                        {
                            //主界面
                            for (int i = 0; i < 16; i++)
                            {
                                (stackPanel_1_Left.Children[i] as TextBlock).Background = Brushes.Transparent;
                                (stackPanel_1_Right.Children[i] as TextBlock).Background = Brushes.Transparent;
                                (stackPanel_1_Mid.Children[i] as TextBlock).Background = Brushes.Transparent;
                                (stackPanel_1_Guard.Children[i] as TextBlock).Background = Brushes.Transparent;
                                (stackPanel_1_Left.Children[i] as TextBlock).Foreground = Brushes.Black;
                                (stackPanel_1_Right.Children[i] as TextBlock).Foreground = Brushes.Black;
                                (stackPanel_1_Mid.Children[i] as TextBlock).Foreground = Brushes.Black;
                                (stackPanel_1_Guard.Children[i] as TextBlock).Foreground = Brushes.Black;

                            }
                        
                        }
                        else if (m_bOwnposdisp == true)
                        {
                           
                            for (int i = 0; i < 14; i++)
                            {
                                (stackPanel_1_Left.Children[i] as TextBlock).Background = Brushes.Transparent;
                                (stackPanel_1_Right.Children[i] as TextBlock).Background = Brushes.Transparent;
                                (stackPanel_1_Mid.Children[i] as TextBlock).Background = Brushes.Transparent;
                                (stackPanel_1_Guard.Children[i] as TextBlock).Background = Brushes.Transparent;
                                (stackPanel_1_Left.Children[i] as TextBlock).Foreground = Brushes.Black;
                                (stackPanel_1_Right.Children[i] as TextBlock).Foreground = Brushes.Black;
                                (stackPanel_1_Mid.Children[i] as TextBlock).Foreground = Brushes.Black;
                                (stackPanel_1_Guard.Children[i] as TextBlock).Foreground = Brushes.Black;

                            }
                        }
                        //DSPL界面
                        for (int i = 0; i < 3; i++)
                        {
                            (stackPanel_2_3_Left.Children[i] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_2_3_Right.Children[i] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_2_3_Mid.Children[i] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_2_3_Guard.Children[i] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_2_3_Left.Children[i] as TextBlock).Foreground = Brushes.Black;
                            (stackPanel_2_3_Right.Children[i] as TextBlock).Foreground = Brushes.Black;
                            (stackPanel_2_3_Mid.Children[i] as TextBlock).Foreground = Brushes.Black;
                            (stackPanel_2_3_Guard.Children[i] as TextBlock).Foreground = Brushes.Black;

                        }
                        break;
                    }
                //左旋或摇杆下键按下,主菜单
                case 1:
                    {
                        TitleDisplay(m_iTitle_Choose);
                        BottomDisplay();
                        //将第一条高亮
                        if (rectangle_ChoseSign_Top.Visibility == Visibility.Visible)
                        {
                            
                            rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                            (stackPanel_1_Left.Children[0] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Right.Children[0] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Mid.Children[0] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Guard.Children[0] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Left.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Right.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Mid.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Guard.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); 
                            m_Count_Choose--;
                         
                        }
                        //绘制高亮
                        else
                        {
                            (stackPanel_1_Left.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Right.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Mid.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Guard.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                            (stackPanel_1_Left.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Right.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_1_Mid.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                            (stackPanel_1_Guard.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                            (stackPanel_1_Left.Children[m_Count_Choose - 1] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_1_Right.Children[m_Count_Choose - 1] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_1_Mid.Children[m_Count_Choose - 1] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_1_Guard.Children[m_Count_Choose - 1] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_1_Left.Children[m_Count_Choose - 1] as TextBlock).Foreground = Brushes.Black;
                            (stackPanel_1_Right.Children[m_Count_Choose - 1] as TextBlock).Foreground = Brushes.Black;
                            (stackPanel_1_Mid.Children[m_Count_Choose - 1] as TextBlock).Foreground = Brushes.Black;
                            (stackPanel_1_Guard.Children[m_Count_Choose - 1] as TextBlock).Foreground = Brushes.Black;
                        }
                        break;
                    }
                //右旋或遥杆上键按下,主菜单
                case 2:
                    {
                        TitleDisplay(m_iTitle_Choose);
                        BottomDisplay();
                        rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                        (stackPanel_1_Left.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                        (stackPanel_1_Right.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                        (stackPanel_1_Mid.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                        (stackPanel_1_Guard.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                        (stackPanel_1_Left.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                        (stackPanel_1_Right.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                        (stackPanel_1_Mid.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                        (stackPanel_1_Guard.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                        (stackPanel_1_Left.Children[m_Count_Choose + 1] as TextBlock).Background = Brushes.Transparent;
                        (stackPanel_1_Right.Children[m_Count_Choose + 1] as TextBlock).Background = Brushes.Transparent;
                        (stackPanel_1_Mid.Children[m_Count_Choose + 1] as TextBlock).Background = Brushes.Transparent;
                        (stackPanel_1_Guard.Children[m_Count_Choose + 1] as TextBlock).Background = Brushes.Transparent;
                        (stackPanel_1_Left.Children[m_Count_Choose + 1] as TextBlock).Foreground = Brushes.Black;
                        (stackPanel_1_Right.Children[m_Count_Choose + 1] as TextBlock).Foreground = Brushes.Black;
                        (stackPanel_1_Mid.Children[m_Count_Choose + 1] as TextBlock).Foreground = Brushes.Black;
                        (stackPanel_1_Guard.Children[m_Count_Choose + 1] as TextBlock).Foreground = Brushes.Black;
                        break;
                    }
                //左旋或摇杆下键按下,DSPL菜单
                case 3:
                    {
                        TitleDisplay(m_iTitle_Choose);
                        BottomDisplay();
                        if (rectangle_ChoseSign_Top.Visibility == Visibility.Visible)
                        {
                            rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                            (stackPanel_2_3_Left.Children[0] as TextBlock).Background = Brushes.Black;
                            (stackPanel_2_3_Right.Children[0] as TextBlock).Background = Brushes.Black;
                            (stackPanel_2_3_Mid.Children[0] as TextBlock).Background = Brushes.Black;
                            (stackPanel_2_3_Left.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_2_3_Right.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_2_3_Mid.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                            m_Count_Choose--;
                        }
                        else//绘制高亮
                        {
                            TitleDisplay(m_iTitle_Choose);
                            BottomDisplay();
                            (stackPanel_2_3_Left.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                            (stackPanel_2_3_Right.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                            (stackPanel_2_3_Mid.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                            (stackPanel_2_3_Left.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_2_3_Right.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                            (stackPanel_2_3_Mid.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                            (stackPanel_2_3_Left.Children[m_Count_Choose - 1] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_2_3_Right.Children[m_Count_Choose - 1] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_2_3_Mid.Children[m_Count_Choose - 1] as TextBlock).Background = Brushes.Transparent;
                            (stackPanel_2_3_Left.Children[m_Count_Choose - 1] as TextBlock).Foreground = Brushes.Black;
                            (stackPanel_2_3_Right.Children[m_Count_Choose - 1] as TextBlock).Foreground = Brushes.Black;
                            (stackPanel_2_3_Mid.Children[m_Count_Choose - 1] as TextBlock).Foreground = Brushes.Black;
                        }
                        break;
                    }
                //右旋或遥杆上键按下,DSPL菜单
                case 4:
                    {
                        TitleDisplay(m_iTitle_Choose);
                        BottomDisplay();
                        rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                        (stackPanel_2_3_Left.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                        (stackPanel_2_3_Right.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                        (stackPanel_2_3_Mid.Children[m_Count_Choose] as TextBlock).Background = Brushes.Black;
                        (stackPanel_2_3_Left.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                        (stackPanel_2_3_Right.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                        (stackPanel_2_3_Mid.Children[m_Count_Choose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                        (stackPanel_2_3_Left.Children[m_Count_Choose + 1] as TextBlock).Background = Brushes.Transparent;
                        (stackPanel_2_3_Right.Children[m_Count_Choose + 1] as TextBlock).Background = Brushes.Transparent;
                        (stackPanel_2_3_Mid.Children[m_Count_Choose + 1] as TextBlock).Background = Brushes.Transparent;
                        (stackPanel_2_3_Left.Children[m_Count_Choose + 1] as TextBlock).Foreground = Brushes.Black;
                        (stackPanel_2_3_Right.Children[m_Count_Choose + 1] as TextBlock).Foreground = Brushes.Black;
                        (stackPanel_2_3_Mid.Children[m_Count_Choose + 1] as TextBlock).Foreground = Brushes.Black;
                        break;
                    }
            }
        }
        //主界面数据滚动(结合高亮）
        private void Roll_Data(int choose)
        {
            if(m_bOwnposdisp==false)
            {
                switch (choose)
                {
                    //对应右旋按键或摇杆上键
                    case 1:
                        {
                            //绘制高亮，主界面
                            if (m_iFunOrder == 1 && m_Count_Choose > 0)
                            {
                                //将上三角标记透明
                                if (m_Count_Choose == 1)
                                {
                                    polygon_ChoseSign_Up.Fill = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                                }
                                else
                                {
                                    polygon_ChoseSign_Up.Fill = Brushes.Black;
                                    polygon_ChoseSign_Down.Fill = Brushes.Black;
                                }


                                m_Count_Choose--;
                                Draw_Choose(2);
                            }
                            else if (m_Count_Choose == 0 && polygon_ChoseSign_Up.Visibility == Visibility.Visible)
                            {
                                polygon_ChoseSign_Up.Fill = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                                //显示下三角标记
                                polygon_ChoseSign_Down.Visibility = Visibility.Visible;
                                //绘制数据向上滚动
                                for (int i = 14; i > -1; i--)
                                {

                                    (stackPanel_1_Left.Children[i + 1] as TextBlock).Text = (stackPanel_1_Left.Children[i] as TextBlock).Text;
                                    (stackPanel_1_Right.Children[i + 1] as TextBlock).Text = (stackPanel_1_Right.Children[i] as TextBlock).Text;
                                    (stackPanel_1_Mid.Children[i + 1] as TextBlock).Text = (stackPanel_1_Mid.Children[i] as TextBlock).Text;
                                }

                                (stackPanel_1_Left.Children[0] as TextBlock).Text = m_strData[m_Count_Choose_Up - 1, 0] + "°:";
                                if (m_iNAME_Disp == 0)
                                {
                                    (stackPanel_1_Right.Children[0] as TextBlock).Text = m_strData[m_Count_Choose_Up - 1, 2];
                                }
                                else if (m_iNAME_Disp == 1)
                                {
                                    (stackPanel_1_Right.Children[0] as TextBlock).Text = m_strData[m_Count_Choose_Up - 1, 3];
                                }
                                (stackPanel_1_Mid.Children[0] as TextBlock).Text = m_strData[m_Count_Choose_Up - 1, 1] + "NM";
                                m_Count_Choose_Up--;
                                m_Count_Choose_Down--;
                                //隐藏上三角标记
                                if (m_Count_Choose == 0 && m_Count_Choose_Up == 0)
                                {
                                    polygon_ChoseSign_Up.Visibility = Visibility.Collapsed;

                                }
                                //隐藏下三角标记
                                if (m_Count_Choose == 15 && m_Count_Choose_Down == m_iTotal - 1)
                                {
                                    polygon_ChoseSign_Down.Visibility = Visibility.Collapsed;

                                }

                            }
                            break;
                        }
                    //对应左旋按键或摇杆下键
                    case 2:
                        {
                            //绘制高亮，主界面
                            if (m_iFunOrder == 1 && m_Count_Choose < 15)
                            {
                                //将下三角标记透明
                                if (m_Count_Choose == 14)
                                {
                                    polygon_ChoseSign_Down.Fill = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                                }
                                //将下三角标记变黑
                                else
                                {
                                    polygon_ChoseSign_Up.Fill = Brushes.Black;
                                    polygon_ChoseSign_Down.Fill = Brushes.Black;
                                }

                                m_Count_Choose++;
                                Draw_Choose(1);
                            }
                            //防止超过船舶总数
                            else if (m_Count_Choose == 15 && m_Count_Choose_Down < m_iTotal - 1)
                            {
                                polygon_ChoseSign_Down.Fill = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                                //显示上三角标记
                                polygon_ChoseSign_Up.Visibility = Visibility.Visible;
                                //绘制数据向上滚动
                                for (int i = 0; i < 15; i++)
                                {
                                    (stackPanel_1_Left.Children[i] as TextBlock).Text = (stackPanel_1_Left.Children[i + 1] as TextBlock).Text;
                                    (stackPanel_1_Right.Children[i] as TextBlock).Text = (stackPanel_1_Right.Children[i + 1] as TextBlock).Text;
                                    (stackPanel_1_Mid.Children[i] as TextBlock).Text = (stackPanel_1_Mid.Children[i + 1] as TextBlock).Text;
                                }
                                //更新最后一行数据
                                (stackPanel_1_Left.Children[15] as TextBlock).Text = m_strData[m_Count_Choose_Down + 1, 0] + "°:";
                                if (m_iNAME_Disp == 0)
                                {
                                    (stackPanel_1_Right.Children[15] as TextBlock).Text = m_strData[m_Count_Choose_Down + 1, 2];
                                }
                                else  if (m_iNAME_Disp == 1)
                                {
                                    (stackPanel_1_Right.Children[15] as TextBlock).Text = m_strData[m_Count_Choose_Down + 1, 3];
                                }
                                (stackPanel_1_Mid.Children[15] as TextBlock).Text = m_strData[m_Count_Choose_Down + 1, 1] + "NM";
                                //更改滚动数据逻辑值
                                m_Count_Choose_Down++;
                                m_Count_Choose_Up++;
                                //隐藏下三角标记
                                if (m_Count_Choose == 15 && m_Count_Choose_Down == m_iTotal - 1)
                                {
                                    polygon_ChoseSign_Down.Visibility = Visibility.Collapsed;

                                }
                            }
                            break;
                        }
                }
            }
            if (m_bOwnposdisp == true)
            {
                 switch (choose)
                {
                    //对应右旋按键或摇杆上键
                    case 1:
                        {
                            //绘制高亮，主界面
                            if (m_iFunOrder == 1 && m_Count_Choose > 0)
                            {
                                //将上三角标记透明
                                if (m_Count_Choose == 1)
                                {
                                    polygon_ChoseSign_Up.Fill = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                                }
                                else
                                {
                                    polygon_ChoseSign_Up.Fill = Brushes.Black;
                                    polygon_ChoseSign_Down_Copy.Fill = Brushes.Black;
                                }


                                m_Count_Choose--;
                                Draw_Choose(2);
                            }
                            else if (m_Count_Choose == 0 && polygon_ChoseSign_Up.Visibility == Visibility.Visible)
                            {
                                polygon_ChoseSign_Up.Fill = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                                //显示下三角标记
                                polygon_ChoseSign_Down_Copy.Visibility = Visibility.Visible;
                                //绘制数据向上滚动
                                for (int i = 12; i > -1; i--)
                                {

                                    (stackPanel_1_Left.Children[i + 1] as TextBlock).Text = (stackPanel_1_Left.Children[i] as TextBlock).Text;
                                    (stackPanel_1_Right.Children[i + 1] as TextBlock).Text = (stackPanel_1_Right.Children[i] as TextBlock).Text;
                                    (stackPanel_1_Mid.Children[i + 1] as TextBlock).Text = (stackPanel_1_Mid.Children[i] as TextBlock).Text;
                                }

                                (stackPanel_1_Left.Children[0] as TextBlock).Text = m_strData[m_Count_Choose_Up - 1, 0] + "°:";
                                if (m_iNAME_Disp == 0)
                                {
                                    (stackPanel_1_Right.Children[0] as TextBlock).Text = m_strData[m_Count_Choose_Up - 1, 2];
                                }
                                else if (m_iNAME_Disp == 1)
                                {
                                    (stackPanel_1_Right.Children[0] as TextBlock).Text = m_strData[m_Count_Choose_Up - 1, 3];
                                }
                                (stackPanel_1_Mid.Children[0] as TextBlock).Text = m_strData[m_Count_Choose_Up - 1, 1] + "NM";
                                m_Count_Choose_Up--;
                                m_Count_Choose_Down--;
                                //隐藏上三角标记
                                if (m_Count_Choose == 0 && m_Count_Choose_Up == 0)
                                {
                                    polygon_ChoseSign_Up.Visibility = Visibility.Collapsed;

                                }

                                //隐藏下三角标记
                                if (m_Count_Choose == 13 && m_Count_Choose_Down == m_iTotal - 1)
                                {
                                    polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;

                                }

                            }
                            break;
                        }
                    //对应左旋按键或摇杆下键
                    case 2:
                        {
                            //绘制高亮，主界面
                            if (m_iFunOrder == 1 && m_Count_Choose < 13)
                            {
                                //将下三角标记透明
                                if (m_Count_Choose == 12)
                                {
                                    polygon_ChoseSign_Down_Copy.Fill = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                                }
                                //将下三角标记变黑
                                else
                                {
                                    polygon_ChoseSign_Up.Fill = Brushes.Black;
                                    polygon_ChoseSign_Down_Copy.Fill = Brushes.Black;
                                }

                                m_Count_Choose++;
                                Draw_Choose(1);
                            }
                            //防止超过船舶总数
                            else if (m_Count_Choose == 13 && m_Count_Choose_Down < m_iTotal - 1)
                            {
                                polygon_ChoseSign_Down_Copy.Fill = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                                //显示上三角标记
                                polygon_ChoseSign_Up.Visibility = Visibility.Visible;
                                //绘制数据向上滚动
                                for (int i = 0; i < 13; i++) 
                                {
                                    (stackPanel_1_Left.Children[i] as TextBlock).Text = (stackPanel_1_Left.Children[i + 1] as TextBlock).Text;
                                    (stackPanel_1_Right.Children[i] as TextBlock).Text = (stackPanel_1_Right.Children[i + 1] as TextBlock).Text;
                                    (stackPanel_1_Mid.Children[i] as TextBlock).Text = (stackPanel_1_Mid.Children[i + 1] as TextBlock).Text;
                                }
                                //更新最后一行数据
                                (stackPanel_1_Left.Children[13] as TextBlock).Text = m_strData[m_Count_Choose_Down + 1, 0] + "°:";
                                if(m_iNAME_Disp==0)
                                {
                                (stackPanel_1_Right.Children[13] as TextBlock).Text = m_strData[m_Count_Choose_Down + 1, 2];
                                }
                                else if(m_iNAME_Disp==1)
                                {
                                (stackPanel_1_Right.Children[13] as TextBlock).Text = m_strData[m_Count_Choose_Down + 1, 3];
                                }
                                (stackPanel_1_Mid.Children[13] as TextBlock).Text = m_strData[m_Count_Choose_Down + 1, 1] + "NM";
                                //更改滚动数据参考值
                                m_Count_Choose_Down++;
                                m_Count_Choose_Up++;
                                //隐藏下三角标记
                                if (m_Count_Choose == 13 && m_Count_Choose_Down == m_iTotal - 1)
                                {
                                    polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;

                                }
                            }
                            break;
                        }
                }
            
            }
        }
        //定义图形显示界面船舶选择逻辑值
        int m_DispChoose = 0;
        //图形显示界面船舶选择
        private void Disp_ShipChoose(int choose)
        {
            (stackPanel_2_3_Left.Children[0] as TextBlock).Text = m_strData[choose, 0] + "°:";
            (stackPanel_2_3_Mid.Children[0] as TextBlock).Text = m_strData[choose, 1] + "NM";
            (stackPanel_2_3_Right.Children[0] as TextBlock).Text = m_strData[choose, 2];
            (stackPanel_2_3_Left.Children[1] as TextBlock).Text = m_strData[choose+1, 0] + "°:";
            (stackPanel_2_3_Mid.Children[1] as TextBlock).Text = m_strData[choose+1, 1] + "NM";
            (stackPanel_2_3_Right.Children[1] as TextBlock).Text = m_strData[choose+1, 2];
            (stackPanel_2_3_Left.Children[2] as TextBlock).Text = m_strData[choose+2, 0] + "°:";
            (stackPanel_2_3_Mid.Children[2] as TextBlock).Text = m_strData[choose+2, 1] + "NM";
            (stackPanel_2_3_Right.Children[2] as TextBlock).Text = m_strData[choose+2, 2];
            (stackPanel_2_3_Left.Children[0] as TextBlock).Background = Brushes.Black;
            (stackPanel_2_3_Right.Children[0] as TextBlock).Background = Brushes.Black;
            (stackPanel_2_3_Mid.Children[0] as TextBlock).Background = Brushes.Black;
            (stackPanel_2_3_Guard.Children[0] as TextBlock).Background = Brushes.Black;
            (stackPanel_2_3_Left.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
            (stackPanel_2_3_Right.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
            (stackPanel_2_3_Mid.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
            (stackPanel_2_3_Guard.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
      
        }
        //设置界面逻辑值
        int m_iSelect = 0;
        //输入（虚拟键盘）逻辑值
        int m_iKeyBoard_Sel = 0;
        //设置界面绘制高亮
        private void Draw_Select()
        {
            //设置界面操作
            if (m_iFunOrder == 4)
            {
                for (int i = 0; i < 6; i++)
                {
                    (grid_4_Down.Children[i] as Rectangle).Visibility = Visibility.Collapsed;
                    (warpPanel_4_Down.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (grid_4_Down.Children[m_iSelect] as Rectangle).Visibility = Visibility.Visible;
                    (warpPanel_4_Down.Children[m_iSelect] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                }
            }
            //List界面
            else  if (m_iFunOrder == 41)
            {
                for (int i = 0; i < 3; i++)
                {
                    (grid_4_Down_List_Rec.Children[i] as Rectangle).Visibility = Visibility.Collapsed;
                    (warpPanel_4_Down_List.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (grid_4_Down_List_Rec.Children[m_iList_Select] as Rectangle).Visibility = Visibility.Visible;
                    (warpPanel_4_Down_List.Children[m_iList_Select] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                }
                for (int i = 3; i < 10; i++)
                {
                    (grid_4_Down_List_Rec.Children[i] as Rectangle).Visibility = Visibility.Collapsed;
                }
                for (int i = 0; i < 2; i++)
                {
                    (warpPanel_4_Down_BRG.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (warpPanel_4_Down_NAME.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (warpPanel_4_Down_SORT.Children[i] as TextBlock).Foreground = Brushes.Black;
                }
                (warpPanel_4_Down_SORT.Children[2] as TextBlock).Foreground = Brushes.Black;
            }
            //List界面,BRG选项
            else   if (m_iFunOrder == 411)
             {
                 for (int i = 0; i < 3; i++)
                 {
                     
                     (warpPanel_4_Down_List.Children[i] as TextBlock).Foreground = Brushes.Black;
                     (grid_4_Down_List_Rec.Children[i] as Rectangle).Visibility = Visibility.Collapsed;            
                 }
                 (grid_4_Down_List_Rec.Children[m_iList_Select] as Rectangle).Visibility = Visibility.Visible;
                 if (m_iList_Select == 3)
                 {
                     (grid_4_Down_List_Rec.Children[4] as Rectangle).Visibility = Visibility.Collapsed;  
                     (warpPanel_4_Down_BRG.Children[1] as TextBlock).Foreground = Brushes.Black;
                   (warpPanel_4_Down_BRG.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                 }
                 else if (m_iList_Select == 4)
                 {
                     (grid_4_Down_List_Rec.Children[3] as Rectangle).Visibility = Visibility.Collapsed;  
                     (warpPanel_4_Down_BRG.Children[0] as TextBlock).Foreground = Brushes.Black;
                     (warpPanel_4_Down_BRG.Children[1] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                 }
              }
            //List界面,SORT选项
            else  if (m_iFunOrder == 412)
               {
                   for (int i = 0; i < 3; i++)
                   {

                       (warpPanel_4_Down_List.Children[i] as TextBlock).Foreground = Brushes.Black;
                       (grid_4_Down_List_Rec.Children[i] as Rectangle).Visibility = Visibility.Collapsed;
                   }
                   (grid_4_Down_List_Rec.Children[m_iList_Select] as Rectangle).Visibility = Visibility.Visible;
                   if (m_iList_Select == 5)
                   {
                       (grid_4_Down_List_Rec.Children[6] as Rectangle).Visibility = Visibility.Collapsed;  
                       (warpPanel_4_Down_SORT.Children[1] as TextBlock).Foreground = Brushes.Black;
                       (warpPanel_4_Down_SORT.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                   }
                   else if (m_iList_Select == 6)
                   {
                       (grid_4_Down_List_Rec.Children[5] as Rectangle).Visibility = Visibility.Collapsed;
                       (grid_4_Down_List_Rec.Children[7] as Rectangle).Visibility = Visibility.Collapsed;  
                       (warpPanel_4_Down_SORT.Children[0] as TextBlock).Foreground = Brushes.Black;
                       (warpPanel_4_Down_SORT.Children[2] as TextBlock).Foreground = Brushes.Black;
                       (warpPanel_4_Down_SORT.Children[1] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                   }
                   else if (m_iList_Select == 7)
                   {
                       (grid_4_Down_List_Rec.Children[6] as Rectangle).Visibility = Visibility.Collapsed;  
                       (warpPanel_4_Down_SORT.Children[1] as TextBlock).Foreground = Brushes.Black;
                       (warpPanel_4_Down_SORT.Children[2] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                   }

               }
            //List界面,NAME选项
               else if (m_iFunOrder == 413)
               {
                   for (int i = 0; i < 3; i++)
                   {

                       (warpPanel_4_Down_List.Children[i] as TextBlock).Foreground = Brushes.Black;
                       (grid_4_Down_List_Rec.Children[i] as Rectangle).Visibility = Visibility.Collapsed;
                   }
                   (grid_4_Down_List_Rec.Children[m_iList_Select] as Rectangle).Visibility = Visibility.Visible;
                   if (m_iList_Select == 8)
                   {
                       (grid_4_Down_List_Rec.Children[9] as Rectangle).Visibility = Visibility.Collapsed;  
                       (warpPanel_4_Down_NAME.Children[1] as TextBlock).Foreground = Brushes.Black;
                       (warpPanel_4_Down_NAME.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                   }
                   else if (m_iList_Select == 9)
                   {
                       (grid_4_Down_List_Rec.Children[8] as Rectangle).Visibility = Visibility.Collapsed;  
                       (warpPanel_4_Down_NAME.Children[0] as TextBlock).Foreground = Brushes.Black;
                       (warpPanel_4_Down_NAME.Children[1] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                   }
               
            



            }
        }
        //本船信息主界面显示设置
        private void OwnposDisp_Select()
        {

            for (int i = 0; i < 2; i++)
            {
                (grid_4_Down_Ownship.Children[i] as Rectangle).Visibility = Visibility.Collapsed;
                (warpPanel_4_Down_Owndisp.Children[i] as TextBlock).Foreground = Brushes.Black;
                (grid_4_Down_Ownship.Children[m_iOwnpos] as Rectangle).Visibility = Visibility.Visible;
                (warpPanel_4_Down_Owndisp.Children[m_iOwnpos] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
            }



        }
        
        //输入（虚拟键盘）选择及高亮
        private void Sel_KeyBoard()
        {
            if(m_iFunOrder==5)
            {
            for (int i = 0; i < 66; i++)
            {
                
                (warpPanel_5_Input.Children[i] as TextBlock).Foreground = Brushes.Black;
                (warpPanel_5_Input.Children[i] as TextBlock).Background = Brushes.Transparent;
            
            }
            (warpPanel_5_Input.Children[m_iKeyBoard_Sel] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
            (warpPanel_5_Input.Children[m_iKeyBoard_Sel] as TextBlock).Background = Brushes.Black;
            }
            else if(m_iFunOrder==7)
            {
                (stackPanel_Input_Setup.Children[0] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[0] as TextBlock).Background = Brushes.Transparent;
                (stackPanel_Input_Setup.Children[1] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[1] as TextBlock).Background = Brushes.Transparent;
                (stackPanel_Input_Setup.Children[3] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[3] as TextBlock).Background = Brushes.Transparent;
            for (int i = 0; i < 66; i++)
            {
                
                (warpPanel_Input.Children[i] as TextBlock).Foreground = Brushes.Black;
                (warpPanel_Input.Children[i] as TextBlock).Background = Brushes.Transparent;
            
            }
            if (m_iKeyBoard_Sel < 66)
            {
                (warpPanel_Input.Children[m_iKeyBoard_Sel] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (warpPanel_Input.Children[m_iKeyBoard_Sel] as TextBlock).Background = Brushes.Black;
            }
            if (m_iKeyBoard_Sel == 66)
            {
                for (int i = 0; i < 66; i++)
                {

                    (warpPanel_Input.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (warpPanel_Input.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                (stackPanel_Input_Setup.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_Input_Setup.Children[0] as TextBlock).Background = Brushes.Black;
                (stackPanel_Input_Setup.Children[1] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[1] as TextBlock).Background = Brushes.Transparent;
                (stackPanel_Input_Setup.Children[3] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[3] as TextBlock).Background = Brushes.Transparent;
            }
            else if (m_iKeyBoard_Sel == 67)
            {
                (stackPanel_Input_Setup.Children[0] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[0] as TextBlock).Background = Brushes.Transparent;
                (stackPanel_Input_Setup.Children[1] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_Input_Setup.Children[1] as TextBlock).Background = Brushes.Black;
                (stackPanel_Input_Setup.Children[3] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[3] as TextBlock).Background = Brushes.Transparent;
            }
            else if (m_iKeyBoard_Sel == 68)
            {
                (stackPanel_Input_Setup.Children[0] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[0] as TextBlock).Background = Brushes.Transparent;
                (stackPanel_Input_Setup.Children[1] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[1] as TextBlock).Background = Brushes.Transparent;
                (stackPanel_Input_Setup.Children[3] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_Input_Setup.Children[3] as TextBlock).Background = Brushes.Black;
            }
            else if (m_iKeyBoard_Sel > 68)
            {
                m_iKeyBoard_Sel = 68;
                Sel_KeyBoard();
            }
            }
            else if (m_iFunOrder == 70)
            {
                (stackPanel_Input_Setup.Children[0] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[0] as TextBlock).Background = Brushes.Transparent;
                (stackPanel_Input_Setup.Children[1] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_Input_Setup.Children[1] as TextBlock).Background = Brushes.Transparent;
                for (int i = 0; i < 66; i++)
                {

                    (warpPanel_Input.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (warpPanel_Input.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                if (m_iKeyBoard_Sel < 66)
                {
                    (warpPanel_Input.Children[m_iKeyBoard_Sel] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (warpPanel_Input.Children[m_iKeyBoard_Sel] as TextBlock).Background = Brushes.Black;
                }
                if (m_iKeyBoard_Sel == 66)
                {
                    for (int i = 0; i < 66; i++)
                    {

                        (warpPanel_Input.Children[i] as TextBlock).Foreground = Brushes.Black;
                        (warpPanel_Input.Children[i] as TextBlock).Background = Brushes.Transparent;

                    }
                    (stackPanel_Input_Setup.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_Input_Setup.Children[0] as TextBlock).Background = Brushes.Black;
                    (stackPanel_Input_Setup.Children[1] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_Input_Setup.Children[1] as TextBlock).Background = Brushes.Transparent;
                }
                else if (m_iKeyBoard_Sel == 67)
                {
                    (stackPanel_Input_Setup.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_Input_Setup.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_Input_Setup.Children[1] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_Input_Setup.Children[1] as TextBlock).Background = Brushes.Black;
                }
            }
          
        }
        //密码显示位置逻辑值
        int m_iPassword_sel = 0;
        //密码显示高亮
        private void Draw_Password()
        {
            if (m_iPassword_sel < 4)
            {
                for (int i = 0; i < 4; i++)
                {

                    (warpPanel_5_Passcode.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (warpPanel_5_Passcode.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                (warpPanel_5_Passcode.Children[m_iPassword_sel] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (warpPanel_5_Passcode.Children[m_iPassword_sel] as TextBlock).Background = Brushes.Black;
            }
            else if (m_iPassword_sel == 4)
            {
            }
        }
        //图形显示设置逻辑值
        int m_iGrphic_Sel = 0;
        //图形显示设置高亮
        private void Draw_Grphic_Setup()
        {
            for (int i = 0; i < 17; i++)
            {

                (warpPanel_2_Setup.Children[i] as TextBlock).Foreground = Brushes.Black;
                (warpPanel_2_Setup.Children[i] as TextBlock).Background = Brushes.Transparent;

            }
            (warpPanel_2_Setup.Children[m_iGrphic_Sel] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
            (warpPanel_2_Setup.Children[m_iGrphic_Sel] as TextBlock).Background = Brushes.Black;
            //设置改变
            (warpPanel_2_Setup.Children[1] as TextBlock).Text = m_fDispRNG.ToString() + "NM";
            if (m_iBRG_Disp == 0)
            {
                (warpPanel_2_Setup.Children[3] as TextBlock).Text = "NORTH";
            }
            else
            {
                (warpPanel_2_Setup.Children[3] as TextBlock).Text = "HEAD";
            }
            if (m_iSORT_Disp == 0)
            {
                (warpPanel_2_Setup.Children[5] as TextBlock).Text = "RANGE";
            }
            else if (m_iSORT_Disp == 1)
            {
                (warpPanel_2_Setup.Children[5] as TextBlock).Text = "TCPA";
            }
            else if (m_iSORT_Disp == 2)
            {
                (warpPanel_2_Setup.Children[5] as TextBlock).Text = "GROUP";
            }
            (warpPanel_2_Setup.Children[7] as TextBlock).Text = m_fGuardRNG.ToString("0.0") + "NM";
       
            (warpPanel_2_Setup.Children[9] as TextBlock).Text = m_iNumOfShip.ToString();
          
            (warpPanel_2_Setup.Children[11] as TextBlock).Text = m_iContrast.ToString();
         
            if (m_bAutoRNG == false)
            {
                (warpPanel_2_Setup.Children[13] as TextBlock).Text = "OFF";
            }
            else
            {
                (warpPanel_2_Setup.Children[13] as TextBlock).Text = "ON";
            }
        }
        //主菜单逻辑值
        int m_iMainMenu_Sel = 0;
        //主菜单子菜单逻辑值
        int m_iMainMenu_60 = 0;
        int m_iMainMenu_61 = 1;
        int m_iMainMenu_62 = 1;
        int m_iMainMenu_63 = 0;
        int m_iMainMenu_64 = 1;
        //主菜单高亮
        private void Draw_MainMenu()
        {
            if (m_iFunOrder == 6)
            {
                for (int i = 0; i < 11; i++)
                {

                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                (stackPanel_6_Mainmenu.Children[m_iMainMenu_Sel] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_6_Mainmenu.Children[m_iMainMenu_Sel] as TextBlock).Background = Brushes.Black;
            }
            else if (m_iFunOrder == 60)
            {
                if (m_iMainMenu_60 < 10)
                {
                    for (int i = 0; i < 11; i++)
                    {

                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;

                    }
                    (Wrap_panel_OtrSel.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[1] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[2] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (Wrap_panel_OtrSel.Children[1] as TextBlock).Foreground = Brushes.Black;
                    (Wrap_panel_OtrSel.Children[2] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_6_Mainmenu.Children[m_iMainMenu_60] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_6_Mainmenu.Children[m_iMainMenu_60] as TextBlock).Background = Brushes.Black;
                }
                else if (m_iMainMenu_60 < 18)
                {
                    for (int i = 0; i < 11; i++)
                    {

                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;

                    }
                    (Wrap_panel_OtrSel.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[1] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[2] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (Wrap_panel_OtrSel.Children[1] as TextBlock).Foreground = Brushes.Black;
                    (Wrap_panel_OtrSel.Children[2] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_6_Mainmenu.Children[m_iMainMenu_60-9] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_6_Mainmenu.Children[m_iMainMenu_60-9] as TextBlock).Background = Brushes.Black;
                }
                else if (m_iMainMenu_60 == 18)
                {
                    for (int i = 0; i < 11; i++)
                    {

                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;
                    }

                    (Wrap_panel_OtrSel.Children[0] as TextBlock).Background=Brushes.Black;
                    (Wrap_panel_OtrSel.Children[1] as TextBlock).Background=Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[2] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[0] as TextBlock).Foreground=new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (Wrap_panel_OtrSel.Children[1] as TextBlock).Foreground=Brushes.Black;
                    (Wrap_panel_OtrSel.Children[2] as TextBlock).Foreground = Brushes.Black;
                }
                else if (m_iMainMenu_60 == 20)
                {
                    (Wrap_panel_OtrSel.Children[1] as TextBlock).Background = Brushes.Black;
                    (Wrap_panel_OtrSel.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[2] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[1] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (Wrap_panel_OtrSel.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (Wrap_panel_OtrSel.Children[2] as TextBlock).Foreground = Brushes.Black;
                }
                else if (m_iMainMenu_60 == 22)
                {
                    (Wrap_panel_OtrSel.Children[2] as TextBlock).Background = Brushes.Black;
                    (Wrap_panel_OtrSel.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[1] as TextBlock).Background = Brushes.Transparent;
                    (Wrap_panel_OtrSel.Children[2] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (Wrap_panel_OtrSel.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (Wrap_panel_OtrSel.Children[1] as TextBlock).Foreground = Brushes.Black;
                }
                
            }
            else if (m_iFunOrder == 61)
            {
                for (int i = 0; i < 11; i++)
                {

                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                (stackPanel_6_Mainmenu.Children[m_iMainMenu_61] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_6_Mainmenu.Children[m_iMainMenu_61] as TextBlock).Background = Brushes.Black;
            }
            else if (m_iFunOrder == 62)
            {
                for (int i = 0; i < 11; i++)
                {

                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                (stackPanel_6_Mainmenu.Children[m_iMainMenu_62] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_6_Mainmenu.Children[m_iMainMenu_62] as TextBlock).Background = Brushes.Black;
            }
            else if (m_iFunOrder == 63)
            {
                if (m_iMainMenu_63 < 9)
                {
                    for (int i = 0; i < 14; i++)
                    {

                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;

                    }
                    (stackPanel_6_Mainmenu.Children[m_iMainMenu_63] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_6_Mainmenu.Children[m_iMainMenu_63] as TextBlock).Background = Brushes.Black;

                }
                else if (m_iMainMenu_63 < 18)
                {
                    for (int i = 0; i < 14; i++)
                    {

                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;

                    }
                    (stackPanel_6_Mainmenu.Children[m_iMainMenu_63-9] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_6_Mainmenu.Children[m_iMainMenu_63-9] as TextBlock).Background = Brushes.Black;
                }
            }
            else if (m_iFunOrder == 64)
            {
                for (int i = 0; i < 13; i++)
                {

                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                (stackPanel_6_Mainmenu.Children[m_iMainMenu_64] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_6_Mainmenu.Children[m_iMainMenu_64] as TextBlock).Background = Brushes.Black;
            }

        }
     
        //主菜单界面变化
        private void MainMenu_Change()
        {                
            m_iMainMenu_Sel = 0;
            
            m_iMainMenu_61 = 1;
            m_iMainMenu_62 = 1;
            
            m_iMainMenu_64 = 1;
         
            Grid_OtrSel.Visibility = Visibility.Collapsed;
            Grid_TX.Visibility = Visibility.Collapsed;
            Grid_TX_Send.Visibility = Visibility.Collapsed;
            polygon_ChoseSign_Up_Copy.Visibility = Visibility.Collapsed;
            polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
            Grid_WayPoint.Visibility = Visibility.Collapsed;
            Grid_WayPointEdit.Visibility = Visibility.Collapsed;
            Grid_ETA.Visibility = Visibility.Collapsed;
            Grid_InterRogation.Visibility = Visibility.Collapsed;
            BottomDisplay();
            
            //清空所有TextBlock
            for (int i = 0; i < 14; i++)
            {
                (stackPanel_6_Mainmenu.Children[i] as TextBlock).Text = " ";
                (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;
                (stackPanel_6_Mainmenu.Children[i] as TextBlock).TextAlignment=TextAlignment.Left;
            }
            //TextBlock内容显示
            if (m_iFunOrder == 60)
            {
                Grid_OtrSel.Visibility = Visibility.Visible;
                
                (Wrap_panel_OtrSel.Children[0] as TextBlock).Text = "[EXIT]";
                (Wrap_panel_OtrSel.Children[1] as TextBlock).Text = "[ENT]";
                (Wrap_panel_OtrSel.Children[2] as TextBlock).Text = "[DEST. LOAD]";
                TitleDisplay(60);
                //第一页
                if (m_iMainMenu_60 < 10)
                {
                    polygon_ChoseSign_Up_Copy.Visibility = Visibility.Collapsed;
                    polygon_ChoseSign_Down_Copy.Visibility = Visibility.Visible;
                    for (int i = 1; i < 14; i += 2)
                    {
                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).TextAlignment = TextAlignment.Right;
                    }
                    (stackPanel_6_Mainmenu.Children[3] as TextBlock).TextAlignment = TextAlignment.Center;
                    (stackPanel_6_Mainmenu.Children[5] as TextBlock).TextAlignment = TextAlignment.Center;
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).TextAlignment = TextAlignment.Center;
                    (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = "1.NAVIGATIONAL STATUS";
                    //本船航行状态显示
                    if (m_iOwnNavStatus == 0)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "UNDER WAY SAILING";
                        m_strOwnshipData[4] = "UNDER WAY SAILING";
                    }
                    else if (m_iOwnNavStatus == 1)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "UNDER WAY USING ENGINE";
                        m_strOwnshipData[4] = "UNDER WAY USING ENGINE";
                    }
                    else if (m_iOwnNavStatus == 2)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "AT ANCHOR";
                        m_strOwnshipData[4] = "AT ANCHOR";
                    }
                    else if (m_iOwnNavStatus == 3)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "NOT UNDER COMMAND";
                        m_strOwnshipData[4] = "NOT UNDER COMMAND";
                    }
                    else if (m_iOwnNavStatus == 4)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "RESTRICTED MANOEUVRABILITY";
                        m_strOwnshipData[4] = "RESTRICTED MANOEUVRABILITY";
                    }
                    else if (m_iOwnNavStatus == 5)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "CONSTRAINED BY HER DRAUGHT";
                        m_strOwnshipData[4] = "CONSTRAINED BY HER DRAUGHT";
                    }
                    else if (m_iOwnNavStatus == 6)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "MOORED";
                        m_strOwnshipData[4] = "MOORED";
                    }
                    else if (m_iOwnNavStatus == 7)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "AGROUND";
                        m_strOwnshipData[4] = "AGROUND";
                    }
                    else if (m_iOwnNavStatus == 8)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "ENGAGED IN FISHING";
                        m_strOwnshipData[4] = "ENGAGED IN FISHING";
                    }
                    else if (m_iOwnNavStatus == 9)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "RESERVED FOR HSC";
                        m_strOwnshipData[4] = "RESERVED FOR HSC";
                    }
                    else if (m_iOwnNavStatus == 10)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "RESERVED FOR WIG";
                        m_strOwnshipData[4] = "RESERVED FOR WIG";
                    }
                    else if (m_iOwnNavStatus == 11)
                    {
                        (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "NOT DEFINED";
                        m_strOwnshipData[4] = "NOT DEFINED";
                    }

                    (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "2.DESTINATION";
                    (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = m_strOwnshipData[11];
                    (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "3.ETA";
                    (stackPanel_6_Mainmenu.Children[5] as TextBlock).Text = m_strOwnshipData[12] + "  " + m_strOwnshipData[13];
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "4.DRAUGHT";
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = m_strOwnshipData[16] + "M";
                    (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "5.CARGO/STATUS";
                    //本船货物显示
                    if (m_iOwnCargoStatus == 0)
                    {
                        (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "ALL SHIPS OF THIS TYPE";
                        m_strOwnshipData[18] = "ALL SHIPS OF THIS TYPE";
                    }
                    else if (m_iOwnCargoStatus == 1)
                    {
                        (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "CATEGORY A(DG/HP/MP)";
                        m_strOwnshipData[18] = "CATEGORY A(DG/HP/MP)";
                    }
                    else if (m_iOwnCargoStatus == 2)
                    {
                        (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "CATEGORY B(DG/HP/MP)";
                        m_strOwnshipData[18] = "CATEGORY B(DG/HP/MP)";
                    }
                    else if (m_iOwnCargoStatus == 3)
                    {
                        (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "CATEGORY C(DG/HP/MP)";
                        m_strOwnshipData[18] = "CATEGORY C(DG/HP/MP)";
                    }
                    else if (m_iOwnCargoStatus == 4)
                    {
                        (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "CATEGORY D(DG/HP/MP)";
                        m_strOwnshipData[18] = "CATEGORY D(DG/HP/MP)";
                    }
                    else if (m_iOwnCargoStatus == 5)
                    {
                        (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "NO ADDITIONAL INFORMATION";
                        m_strOwnshipData[18] = "NO ADDITIONAL INFORMATION";
                    }

                }
                else if (m_iMainMenu_60 > 9&&m_iMainMenu_60<15)
                {
                    polygon_ChoseSign_Up_Copy.Visibility = Visibility.Visible;
                    polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                    m_iMainMenu_60 = 10;
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).TextAlignment = TextAlignment.Center;
                    (stackPanel_6_Mainmenu.Children[8] as TextBlock).TextAlignment = TextAlignment.Center;
                  

                    (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "6.WAYPOINTS";
                    (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "7.WAYPOINT TEXT";
                    (stackPanel_6_Mainmenu.Children[5] as TextBlock).Text = "8.PERSONS ON BOARD";              
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = m_iPersons.ToString();
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "9.HEIGHT OVER KEEL";
                    (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = m_iHeightKeel.ToString("0.0")+"M";

                }
            }
            else if (m_iFunOrder == 61)
            {
                m_iTXSel = 0;
                TitleDisplay(61);
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = " 1.EDIT AND TX";
                (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = " 2.TX TRAY";
                (stackPanel_6_Mainmenu.Children[5] as TextBlock).Text = " 3.RX TRAY";
                (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = " 4.INTERROGATION";
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = " 5.LONG RANGE";

            }
            else if (m_iFunOrder == 62)
            {
                (stackPanel_6_Mainmenu.Children[2] as TextBlock).TextAlignment = TextAlignment.Center;
                (stackPanel_6_Mainmenu.Children[5] as TextBlock).TextAlignment = TextAlignment.Center;
                TitleDisplay(62);
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = " 1.GUARD ZONE";
                (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = m_fGuardRNG.ToString("0.0") + "NM";
                (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = " 2.LOST TARGET";
                (stackPanel_6_Mainmenu.Children[5] as TextBlock).Text = m_fLostRNG.ToString("0.0") + "NM";
                (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = " 3.USER ALARM HISTORY";
            }
            else if (m_iFunOrder == 63)
            {

                for (int i = 1; i < 13; i += 2)
                {
                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).TextAlignment = TextAlignment.Center;
                }
                for (int i = 9; i < 13; i++)
                {
                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).TextAlignment = TextAlignment.Left;
                }
                
                TitleDisplay(63);
                if (m_iMainMenu_63 < 9)
                {
                    polygon_ChoseSign_Up_Copy.Visibility = Visibility.Collapsed;
                    polygon_ChoseSign_Down_Copy.Visibility = Visibility.Visible;
                    (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = " 1.CONTRAST";
                    (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = m_iContrast.ToString();
                    (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = " 2.LOCAL TIME";
                    if (m_bUTC == false)
                    {
                        (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "ON";
                    }
                    else
                    {
                        (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "OFF";
                    }
                    (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = " 3.REGIONAL CHANEL SETTING";
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = " 4.LONG RANGE RESPONSE";
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "AUTO";

                    if (m_bSound == false)
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.BUZZER         ：OFF";
                    }
                    else
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.BUZZER         ：ON";
                    }

                    (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "   - MESSAGE";
                    (stackPanel_6_Mainmenu.Children[10] as TextBlock).Text = "   - GUARD ZONE ALM";
                    (stackPanel_6_Mainmenu.Children[11] as TextBlock).Text = "   - LOST TARGET ALM";
                    (stackPanel_6_Mainmenu.Children[12] as TextBlock).Text = "   - ALARM";
                }
                else if (m_iMainMenu_63 > 9&&m_iMainMenu_63<16)
                {
                    polygon_ChoseSign_Up_Copy.Visibility = Visibility.Visible;
                    polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                    for (int i = 1; i < 13; i += 2)
                    {
                        (stackPanel_6_Mainmenu.Children[i] as TextBlock).TextAlignment = TextAlignment.Left;
                    }
                    m_iMainMenu_63 = 10;
                    (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = " 6.GROUP SHIP";
                    (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = " 7.CHANNEL SETTING";
                    (stackPanel_6_Mainmenu.Children[5] as TextBlock).Text = " 8.PASSWORD";
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = " 9.POS DISP. SETTING:OFF";

                }
                        



            }

            else if (m_iFunOrder == 64)
            {
                TitleDisplay(64);
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = " 1.SELF DIAGNOSIS";
                (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = " 2.TRX CONDITION";
                (stackPanel_6_Mainmenu.Children[5] as TextBlock).Text = " 3.AIS ALARM";
                (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = " 4.SENSOR STATUS";
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = " 5.POWER ON/OFF LOG";
                (stackPanel_6_Mainmenu.Children[11] as TextBlock).Text = " 6.SOFTWARE VERSION";

            }
            else if (m_iFunOrder == 6)
            {
                TitleDisplay(10);
                (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = " 1.VOYAGE STATIC DATA";
                (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = " 2.MESSAGE";
                (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = " 3.ALARM SETTING";
                (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = " 4.SET UP";
                (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.MAINTENANCE";
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = " -------------------------";
                (stackPanel_6_Mainmenu.Children[10] as TextBlock).Text = " POWER REDUCTION:NORMAL";
            }
            else if (m_iFunOrder == 603)
            {
                Grid_ETA.Visibility = Visibility.Visible;
                m_iETAChoose = 0;
                m_iMonth = 1;
                 m_iDay = 1;
                m_iHour = 0;
                m_iMinute = 0;
                (stackPanel_ETA.Children[0] as TextBlock).Text = "MONTH:";
                (stackPanel_ETA.Children[1] as TextBlock).Text = "DAY:";
                (stackPanel_ETA.Children[2] as TextBlock).Text = "HOUR:";
                (stackPanel_ETA.Children[3] as TextBlock).Text = "MINUTE:";
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "JAN";
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = "01";
                (stackPanel_ETA_Edit.Children[2] as TextBlock).Text = "00";
                (stackPanel_ETA_Edit.Children[3] as TextBlock).Text = "00";


            }
            else if (m_iFunOrder == 606)
            {
                Grid_WayPoint.Visibility = Visibility.Visible;
                TitleDisplay(606);
                (stackPanel_Wp_Num.Children[0] as TextBlock).Text = "NO.";
                (stackPanel_Wp_Num.Children[1] as TextBlock).Text = " 1.";
                (stackPanel_Wp_Num.Children[2] as TextBlock).Text = " 2.";
                (stackPanel_Wp_Num.Children[3] as TextBlock).Text = " 3.";
                (stackPanel_Wp_Num.Children[4] as TextBlock).Text = " 4.";
                (stackPanel_Wp_Num.Children[5] as TextBlock).Text = " 5.";
                (stackPanel_Wp_Pos.Children[0] as TextBlock).Text = "POSITION";
                for (int i = 1; i < 11; i++)
                {
                    (stackPanel_Wp_Pos.Children[i] as TextBlock).Text = "";
                }
                if (m_WpLAT[0] != 0 && m_WpLON[0] != 0)
                {
                    if (m_WpLAT[0] > 0 && m_WpLON[0] > 0)
                    {
                        (stackPanel_Wp_Pos.Children[1] as TextBlock).Text = "N    " + ((int)(m_WpLAT[0] / 60)).ToString("00") + "°" + (m_WpLAT[0] % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[2] as TextBlock).Text = "E   " + ((int)(m_WpLON[0] / 60)).ToString("000") + "°" + (m_WpLON[0] % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[0] > 0 && m_WpLON[0] < 0)
                    {
                        (stackPanel_Wp_Pos.Children[1] as TextBlock).Text = "N    " + ((int)(m_WpLAT[0] / 60)).ToString("00") + "°" + (m_WpLAT[0] % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[2] as TextBlock).Text = "W   " + ((int)(Math.Abs(m_WpLON[0]) / 60)).ToString("000") + "°" + (Math.Abs(m_WpLON[0]) % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[0] < 0 && m_WpLON[0] < 0)
                    {
                        (stackPanel_Wp_Pos.Children[1] as TextBlock).Text = "S    " + ((int)(Math.Abs(m_WpLAT[0]) / 60)).ToString("00") + "°" + (Math.Abs(m_WpLAT[0]) % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[2] as TextBlock).Text = "W   " + ((int)(Math.Abs(m_WpLON[0]) / 60)).ToString("000") + "°" + (Math.Abs(m_WpLON[0]) % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[0] < 0 && m_WpLON[0] > 0)
                    {
                        (stackPanel_Wp_Pos.Children[1] as TextBlock).Text = "S    " + ((int)(Math.Abs(m_WpLAT[0]) / 60)).ToString("00") + "°" + (Math.Abs(m_WpLAT[0]) % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[2] as TextBlock).Text = "E   " + ((int)(m_WpLON[0] / 60)).ToString("000") + "°" + (m_WpLON[0] % 60).ToString("00.000") + "′";
                    }
                }
                if (m_WpLAT[1] != 0 && m_WpLON[1] != 0)
                {
                    if (m_WpLAT[1] > 0 && m_WpLON[1] > 0)
                    {
                        (stackPanel_Wp_Pos.Children[3] as TextBlock).Text = "N    " + ((int)(m_WpLAT[1] / 60)).ToString("00") + "°" + (m_WpLAT[1] % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[4] as TextBlock).Text = "E   " + ((int)(m_WpLON[1] / 60)).ToString("000") + "°" + (m_WpLON[1] % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[1] > 0 && m_WpLON[1] < 0)
                    {
                        (stackPanel_Wp_Pos.Children[3] as TextBlock).Text = "N    " + ((int)(m_WpLAT[1] / 60)).ToString("00") + "°" + (m_WpLAT[1] % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[4] as TextBlock).Text = "W   " + ((int)(Math.Abs(m_WpLON[1]) / 60)).ToString("000") + "°" + (Math.Abs(m_WpLON[1]) % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[1] < 0 && m_WpLON[1] < 0)
                    {
                        (stackPanel_Wp_Pos.Children[3] as TextBlock).Text = "S    " + ((int)(Math.Abs(m_WpLAT[1]) / 60)).ToString("00") + "°" + (Math.Abs(m_WpLAT[1]) % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[4] as TextBlock).Text = "W   " + ((int)(Math.Abs(m_WpLON[1]) / 60)).ToString("000") + "°" + (Math.Abs(m_WpLON[1]) % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[1] < 0 && m_WpLON[1] > 0)
                    {
                        (stackPanel_Wp_Pos.Children[3] as TextBlock).Text = "S    " + ((int)(Math.Abs(m_WpLAT[1]) / 60)).ToString("00") + "°" + (Math.Abs(m_WpLAT[1]) % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[4] as TextBlock).Text = "E   " + ((int)(m_WpLON[1] / 60)).ToString("000") + "°" + (m_WpLON[1] % 60).ToString("00.000") + "′";
                    }
                }
                if (m_WpLAT[2] != 0 && m_WpLON[2] != 0)
                {
                    if (m_WpLAT[2] > 0 && m_WpLON[2] > 0)
                    {
                        (stackPanel_Wp_Pos.Children[5] as TextBlock).Text = "N    " + ((int)(m_WpLAT[2] / 60)).ToString("00") + "°" + (m_WpLAT[2] % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[6] as TextBlock).Text = "E   " + ((int)(m_WpLON[2] / 60)).ToString("000") + "°" + (m_WpLON[2] % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[2] > 0 && m_WpLON[2] < 0)
                    {
                        (stackPanel_Wp_Pos.Children[5] as TextBlock).Text = "N    " + ((int)(m_WpLAT[2] / 60)).ToString("00") + "°" + (m_WpLAT[2] % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[6] as TextBlock).Text = "W   " + ((int)(Math.Abs(m_WpLON[2]) / 60)).ToString("000") + "°" + (Math.Abs(m_WpLON[2]) % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[2] < 0 && m_WpLON[2] < 0)
                    {
                        (stackPanel_Wp_Pos.Children[5] as TextBlock).Text = "S     " + ((int)(Math.Abs(m_WpLAT[2]) / 60)).ToString("00") + "°" + (Math.Abs(m_WpLAT[2]) % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[6] as TextBlock).Text = "W   " + ((int)(Math.Abs(m_WpLON[2]) / 60)).ToString("000") + "°" + (Math.Abs(m_WpLON[2]) % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[2] < 0 && m_WpLON[2] > 0)
                    {
                        (stackPanel_Wp_Pos.Children[5] as TextBlock).Text = "S    " + ((int)(Math.Abs(m_WpLAT[2]) / 60)).ToString("00") + "°" + (Math.Abs(m_WpLAT[2]) % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[6] as TextBlock).Text = "E   " + ((int)(m_WpLON[2] / 60)).ToString("000") + "°" + (m_WpLON[2] % 60).ToString("00.000") + "′";
                    }
                }
                if (m_WpLAT[3] != 0 && m_WpLON[3] != 0)
                {
                    if (m_WpLAT[3] > 0 && m_WpLON[3] > 0)
                    {
                        (stackPanel_Wp_Pos.Children[7] as TextBlock).Text = "N    " + ((int)(m_WpLAT[3] / 60)).ToString("00") + "°" + (m_WpLAT[3] % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[8] as TextBlock).Text = "E   " + ((int)(m_WpLON[3] / 60)).ToString("000") + "°" + (m_WpLON[3] % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[3] > 0 && m_WpLON[3] < 0)
                    {
                        (stackPanel_Wp_Pos.Children[7] as TextBlock).Text = "N    " + ((int)(m_WpLAT[3] / 60)).ToString("00") + "°" + (m_WpLAT[3] % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[8] as TextBlock).Text = "W   " + ((int)(Math.Abs(m_WpLON[3]) / 60)).ToString("000") + "°" + (Math.Abs(m_WpLON[3]) % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[3] < 0 && m_WpLON[3] < 0)
                    {
                        (stackPanel_Wp_Pos.Children[7] as TextBlock).Text = "S    " + ((int)(Math.Abs(m_WpLAT[3]) / 60)).ToString("00") + "°" + (Math.Abs(m_WpLAT[3]) % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[8] as TextBlock).Text = "W   " + ((int)(Math.Abs(m_WpLON[3]) / 60)).ToString("000") + "°" + (Math.Abs(m_WpLON[3]) % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[3] < 0 && m_WpLON[3] > 0)
                    {
                        (stackPanel_Wp_Pos.Children[7] as TextBlock).Text = "S    " + ((int)(Math.Abs(m_WpLAT[3]) / 60)).ToString("00") + "°" + (Math.Abs(m_WpLAT[3]) % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[8] as TextBlock).Text = "E   " + ((int)(m_WpLON[3] / 60)).ToString("000") + "°" + (m_WpLON[3] % 60).ToString("00.000") + "′";
                    }
                }
                if (m_WpLAT[4] != 0 && m_WpLON[4] != 0)
                {
                    if (m_WpLAT[4] > 0 && m_WpLON[4] > 0)
                    {
                        (stackPanel_Wp_Pos.Children[9] as TextBlock).Text = "N    " + ((int)(m_WpLAT[4] / 60)).ToString("00") + "°" + (m_WpLAT[4] % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[10] as TextBlock).Text = "E   " + ((int)(m_WpLON[4] / 60)).ToString("000") + "°" + (m_WpLON[4] % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[4] > 0 && m_WpLON[4] < 0)
                    {
                        (stackPanel_Wp_Pos.Children[9] as TextBlock).Text = "N    " + ((int)(m_WpLAT[4] / 60)).ToString("00") + "°" + (m_WpLAT[4] % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[10] as TextBlock).Text = "W   " + ((int)(Math.Abs(m_WpLON[4]) / 60)).ToString("000") + "°" + (Math.Abs(m_WpLON[4]) % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[4] < 0 && m_WpLON[4] < 0)
                    {
                        (stackPanel_Wp_Pos.Children[9] as TextBlock).Text = "S    " + ((int)(Math.Abs(m_WpLAT[4]) / 60)).ToString("00") + "°" + (Math.Abs(m_WpLAT[4]) % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[10] as TextBlock).Text = "W   " + ((int)(Math.Abs(m_WpLON[4]) / 60)).ToString("000") + "°" + (Math.Abs(m_WpLON[4]) % 60).ToString("00.000") + "′";
                    }
                    else if (m_WpLAT[4] < 0 && m_WpLON[4] > 0)
                    {
                        (stackPanel_Wp_Pos.Children[9] as TextBlock).Text = "S    " + ((int)(Math.Abs(m_WpLAT[4]) / 60)).ToString("00") + "°" + (Math.Abs(m_WpLAT[4]) % 60).ToString("00.000") + "′";
                        (stackPanel_Wp_Pos.Children[10] as TextBlock).Text = "E   " + ((int)(m_WpLON[4] / 60)).ToString("000") + "°" + (m_WpLON[4] % 60).ToString("00.000") + "′";
                    }
                }
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Text = "[EXIT]";
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Text = "[SCROLL]";
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Text = "[SAVE]";
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Text = "[ALL CLEAR]";
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Text = "[REVERSE]";
            }
            else if (m_iFunOrder == 6060)
            {
                m_iWpEditChoose = 0;
                m_iLAT_Degree_Ten = 0;
                m_iLAT_Degree_Unit = 0;
                m_iLAT_Minute_Ten = 0;
                m_iLAT_Minute_Unit = 0;
                m_iLAT_Minute_Tenths = 0;
                m_iLAT_Minute_Percent = 0;
                m_iLAT_Minute_Thousands = 0;
                m_iLON_Degree_Hundred = 0;
                m_iLON_Degree_Ten = 0;
                m_iLON_Degree_Unit = 0;
                m_iLON_Minute_Ten = 0;
                m_iLON_Minute_Unit = 0;
                m_iLON_Minute_Tenths = 0;
                m_iLON_Minute_Percent = 0;
                m_iLON_Minute_Thousands = 0;
                Grid_WayPointEdit.Visibility = Visibility.Visible;
                (warppanel_WayPointEditSybl.Children[0] as TextBlock).Text = "°";
                (warppanel_WayPointEditSybl.Children[1] as TextBlock).Text = ".";
                (warppanel_WayPointEditSybl.Children[2] as TextBlock).Text = "′";
                (warppanel_WayPointEditSybl.Children[3] as TextBlock).Text = "°";
                (warppanel_WayPointEditSybl.Children[4] as TextBlock).Text = ".";
                (warppanel_WayPointEditSybl.Children[5] as TextBlock).Text = "′";

                (warppanel_WayPointEdit.Children[0] as TextBlock).Text = "N";
                (warppanel_WayPointEdit.Children[1] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[2] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[3] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[4] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[5] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[6] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[7] as TextBlock).Text = "0";

                (warppanel_WayPointEdit.Children[8] as TextBlock).Text = "E";
                (warppanel_WayPointEdit.Children[9] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[10] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[11] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[12] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[13] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[14] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[15] as TextBlock).Text = "0";
                (warppanel_WayPointEdit.Children[16] as TextBlock).Text = "0";

            }
            else if (m_iFunOrder == 610)
            {

                Grid_TX.Visibility = Visibility.Visible;
                TitleDisplay(610);
                (stackPanel_TX.Children[0] as TextBlock).Text = "1.FORMAT : " + "ADDRESSED";
                (stackPanel_TX.Children[1] as TextBlock).Text = "    MMSI : " + m_strData[m_iMMSiSel, 3];
                (stackPanel_TX.Children[2] as TextBlock).Text = "2.CATAGORY : " + "ROUTINE";
                (stackPanel_TX.Children[3] as TextBlock).Text = "3.FUNCTION : ";
                (stackPanel_TX.Children[4] as TextBlock).Text = "TEXT";
                (stackPanel_TX.Children[5] as TextBlock).Text = "4.REPLY : " + "ON";
                (stackPanel_TX.Children[7] as TextBlock).Text = "5.CH : " + "A/B";
                (stackPanel_TX.Children[9] as TextBlock).Text = "6.NUMBER OF RETRY : " + "3";

                (stackPanel_TXSel.Children[0] as TextBlock).Text = "[EXIT]";
                (stackPanel_TXSel.Children[1] as TextBlock).Text = "[SAVE]";
                (stackPanel_TXSel.Children[2] as TextBlock).Text = "[EDIT]";

            }
            else if (m_iFunOrder == 611)
            {
                Grid_TX_Send.Visibility = Visibility.Visible;
                m_iTXSend_Sel = 0;
                TitleDisplay(611);
                for (int i = 0; i < 10; i++)
                {

                    (stackPanel_TX_Send.Children[i] as TextBlock).Text = "";

                }
                for (int i = 0; i < 6; i++)
                {
                    (warppanel_TX_Send.Children[0] as TextBlock).Text = "";
                }
                TX_Send_Receive_Disp();
                for (int i = 0; i < 10; i++)
                {
                    (stackPanel_TX_Send.Children[i] as TextBlock).Text = (i + 1).ToString() + "." + m_strMessage[i, 0];
                }

            }
            else if (m_iFunOrder == 612)
            {
                Grid_TX_Send.Visibility = Visibility.Visible;
                m_iTXSend_Sel = 0;
                TitleDisplay(612);
                for (int i = 0; i < 10; i++)
                {

                    (stackPanel_TX_Send.Children[i] as TextBlock).Text = "";

                }
                for (int i = 0; i < 6; i++)
                {
                    (warppanel_TX_Send.Children[0] as TextBlock).Text = "";
                }
                TX_Send_Receive_Disp();
                for (int i = 0; i < 10; i++)
                {
                    (stackPanel_TX_Send.Children[i] as TextBlock).Text = (i + 1).ToString() + "."+m_strMessageReceive[i, 0];
                }

            }
            else if (m_iFunOrder == 6111)
            {
                Grid_TX_Send.Visibility = Visibility.Visible;
                for (int i = 0; i < 10; i++)
                {
                    (stackPanel_TX_Send.Children[i] as TextBlock).Text = "";
                    (stackPanel_TX_Send.Children[i] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_TX_Send.Children[i] as TextBlock).Foreground = Brushes.Black;
                }
                (stackPanel_TX_Send.Children[0] as TextBlock).Text = m_strMessage[m_iTXSend_Sel, 7];
            }
            else if (m_iFunOrder == 6121)
            {
                Grid_TX_Send.Visibility = Visibility.Visible;
                for (int i = 0; i < 10; i++)
                {
                    (stackPanel_TX_Send.Children[i] as TextBlock).Text = "";
                    (stackPanel_TX_Send.Children[i] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_TX_Send.Children[i] as TextBlock).Foreground = Brushes.Black;
                }
                (stackPanel_TX_Send.Children[0] as TextBlock).Text = m_strMessageReceive[m_iTXSend_Sel, 7];
            }
            else if (m_iFunOrder == 613)
            {
                m_iIntRogationSel = 0;
                TitleDisplay(613);
                Grid_InterRogation.Visibility = Visibility.Visible;
                (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = "1.DESTINATION ID:"+ m_strData[0, 3];
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "REQUEST1:";
                (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "POSITION REPORT(A)";
                (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "REQUEST2:";
                (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "NONE";

                (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "2.DESTINATION ID:" + m_strData[1, 3];
                (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "REQUEST1:";
                (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "NONE";
                (Wrappanel_InterRogation.Children[0] as TextBlock).Text = "[EXIT]";
                (Wrappanel_InterRogation.Children[1] as TextBlock).Text = "[TX]";
                (Wrappanel_InterRogation.Children[2] as TextBlock).Text = "[CLEAR]";
                (Wrappanel_InterRogation.Children[3] as TextBlock).Text = "[CHK1-1]";
                (Wrappanel_InterRogation.Children[4] as TextBlock).Text = "[CHK1-2]";
                (Wrappanel_InterRogation.Children[5] as TextBlock).Text = "[CHK2]";
               


            }
            else if (m_iFunOrder == 614)
            {
                TitleDisplay(614);
                (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = "A=NAME.CALL SIGN.IMO NO";
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "B=DATE AND TIME MESSAGE";

                (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "C=POSITON";
                (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "E=COG";
                (stackPanel_6_Mainmenu.Children[5] as TextBlock).Text = "F=SOG";
                (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "I=DESTINATION AND ETA";
                (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "O=DRAUGHT";
                (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "P=SHIP/CARGO";
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "U=LENGTH.BREADTH.TYPE";
                (stackPanel_6_Mainmenu.Children[10] as TextBlock).Text = "W=PERSONS ON BOARD";
            }
            else if (m_iFunOrder == 622)
            {
                TitleDisplay(62);
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = " 1." + DateTime.Now.ToString("yyyy/MM/dd") + " " + DateTime.Now.ToString("HH:mm") + " GUARD";
                (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = " 2." + DateTime.Now.ToString("yyyy/MM/dd") + " " + DateTime.Now.ToString("HH:mm") + " LOST";

            }
            else if (m_iFunOrder == 640)
            {
                TitleDisplay(640);
                (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = "1.TRANSPONDER:TEST ALL";
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "             :ENT";
                (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "  [RESULT]   :OK";
                (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "      INT GPS:OK";
                (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "          TRX:OK";
                (stackPanel_6_Mainmenu.Children[5] as TextBlock).Text = "           PS:OK";
                (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "      ANTENNA:INTERNAL";
                (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "2.CONTROLLOR :OK";
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "  [RESULT]   :OK";
                (stackPanel_6_Mainmenu.Children[11] as TextBlock).Text = "3.CONNECTION BOX :ENT";
                (stackPanel_6_Mainmenu.Children[12] as TextBlock).Text = "  [RESULT]   :OK";
            }
            else if (m_iFunOrder == 641)
            {
                TitleDisplay(641);
                (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = "1.";
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "CH A      : 2087 WIDE";
                (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "CH B      : 2088 WIDE";
                (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "TX/RX MODE: TX/RX, TX/AX";
                (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "            (CH A, CH B)";
                (stackPanel_6_Mainmenu.Children[5] as TextBlock).Text = "TX POWER  : HIGH";
                (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "ZONE SIZE : 5NM";
                (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "AREA (NE) : N   °   ,  ′";
                (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "AREA (NE) : E   °   ,  ′";
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "AREA (SW) : N   °   ,  ′";
                (stackPanel_6_Mainmenu.Children[10] as TextBlock).Text = "AREA (SW) : E   °   ,  ′";
                (stackPanel_6_Mainmenu.Children[11] as TextBlock).Text = "SOURCE:";
                (stackPanel_6_Mainmenu.Children[12] as TextBlock).Text = "MMSI  :";
                (stackPanel_6_Mainmenu.Children[13] as TextBlock).Text = "UTC   :     /  /    :";
            }
            else if (m_iFunOrder == 642)
            {
                TitleDisplay(642);
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "  NO DATA";
            }
            else if (m_iFunOrder == 643)
            {
                TitleDisplay(643);
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "  POSITION :";
                (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "   EXTERNAL DGNSS";
                (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "  UTC CLOCK:IN USE";
                (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "  SOG/COG  :EXTERNAL";
                (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "  HEADING  :VALID";
                (stackPanel_6_Mainmenu.Children[10] as TextBlock).Text = "  ROT      :OTHER SORCE";

            }
            else if (m_iFunOrder == 644)
            {
                TitleDisplay(644);
            }
            else if (m_iFunOrder == 645)
            {
                TitleDisplay(645);
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "TRANSPONDER CONT  : 2.00";
                (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "CONTROLLER        : 2.00";
                (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "CONNECTION BOX    : 1.03";
            }

            
        }
        //NAVSTATUS设置
        private void NavStatus_Setup()
        {
            if (m_iOwnNavStatus == 0)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "UNDER WAY SAILING";
                m_strOwnshipData[4] = "UNDER WAY SAILING";
            }
            else if (m_iOwnNavStatus == 1)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "UNDER WAY USING ENGINE";
                m_strOwnshipData[4] = "UNDER WAY USING ENGINE";
            }
            else if (m_iOwnNavStatus == 2)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "AT ANCHOR";
                m_strOwnshipData[4] = "AT ANCHOR";
            }
            else if (m_iOwnNavStatus == 3)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "NOT UNDER COMMAND";
                m_strOwnshipData[4] = "NOT UNDER COMMAND";
            }
            else if (m_iOwnNavStatus == 4)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "RESTRICTED MANOEUVRABILITY";
                m_strOwnshipData[4] = "RESTRICTED MANOEUVRABILITY";
            }
            else if (m_iOwnNavStatus == 5)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "CONSTRAINED BY HER DRAUGHT";
                m_strOwnshipData[4] = "CONSTRAINED BY HER DRAUGHT";
            }
            else if (m_iOwnNavStatus == 6)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "MOORED";
                m_strOwnshipData[4] = "MOORED";
            }
            else if (m_iOwnNavStatus == 7)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "AGROUND";
                m_strOwnshipData[4] = "AGROUND";
            }
            else if (m_iOwnNavStatus == 8)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "ENGAGED IN FISHING";
                m_strOwnshipData[4] = "ENGAGED IN FISHING";
            }
            else if (m_iOwnNavStatus == 9)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "RESERVED FOR HSC";
                m_strOwnshipData[4] = "RESERVED FOR HSC";
            }
            else if (m_iOwnNavStatus == 10)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "RESERVED FOR WIG";
                m_strOwnshipData[4] = "RESERVED FOR WIG";
            }
            else if (m_iOwnNavStatus == 11)
            {
                (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = "NOT DEFINED";
                m_strOwnshipData[4] = "NOT DEFINED";
            }
        }
        //CARGO STATUS设置
        private void CargoStatus_Setup()
        {
            if (m_iOwnCargoStatus == 0)
            {
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "ALL SHIPS OF THIS TYPE";
                m_strOwnshipData[18] = "ALL SHIPS OF THIS TYPE";
            }
            else if (m_iOwnCargoStatus == 1)
            {
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "CATEGORY A(DG/HP/MP)";
                m_strOwnshipData[18] = "CATEGORY A(DG/HP/MP)";
            }
            else if (m_iOwnCargoStatus == 2)
            {
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "CATEGORY B(DG/HP/MP)";
                m_strOwnshipData[18] = "CATEGORY B(DG/HP/MP)";
            }
            else if (m_iOwnCargoStatus == 3)
            {
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "CATEGORY C(DG/HP/MP)";
                m_strOwnshipData[18] = "CATEGORY C(DG/HP/MP)";
            }
            else if (m_iOwnCargoStatus == 4)
            {
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "CATEGORY D(DG/HP/MP)";
                m_strOwnshipData[18] = "CATEGORY D(DG/HP/MP)";
            }
            else if (m_iOwnCargoStatus == 5)
            {
                (stackPanel_6_Mainmenu.Children[9] as TextBlock).Text = "NO ADDITIONAL INFORMATION";
                m_strOwnshipData[18] = "NO ADDITIONAL INFORMATION";
            }
        }
        //ETA设置高亮参数
        int m_iETAChoose = 0;
        //ETA设置高亮
        private void Draw_ETA()
        {
            for (int i = 0; i < 4; i++)
            {

                (stackPanel_ETA_Edit.Children[i] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_ETA_Edit.Children[i] as TextBlock).Background = Brushes.Transparent;

            }
            (stackPanel_ETA_Edit.Children[m_iETAChoose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
            (stackPanel_ETA_Edit.Children[m_iETAChoose] as TextBlock).Background = Brushes.Black;
        }
        //ETA参数
        int m_iMonth = 1;
        int m_iDay = 1;
        int m_iHour = 0;
        int m_iMinute = 0;
        //ETA编辑
        private void ETA_Edit()
        {
            if (m_iMonth == 1)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "JAN";
                if (m_iDay > 31)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 31;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 2)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "FEB";
                if (m_iDay > 29)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 29;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 3)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "MAR";
                if (m_iDay > 31)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 31;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 4)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "APR";
                if (m_iDay > 30)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 30;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 5)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "MAY";
                if (m_iDay > 31)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 31;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 6)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "JUN";
                if (m_iDay > 30)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 30;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 7)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "JUL";
                if (m_iDay > 31)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 31;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 8)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "AUG";
                if (m_iDay > 31)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 31;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 9)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "SEP";
                if (m_iDay > 30)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 30;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 10)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "OCT";
                if (m_iDay > 31)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 31;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 11)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "NOV";
                if (m_iDay > 30)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 30;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            else if (m_iMonth == 12)
            {
                (stackPanel_ETA_Edit.Children[0] as TextBlock).Text = "DEC";
                if (m_iDay > 31)
                {
                    m_iDay = 1;
                }
                if (m_iDay < 1)
                {
                    m_iDay = 31;
                }
                (stackPanel_ETA_Edit.Children[1] as TextBlock).Text = m_iDay.ToString("00");
            }
            

        }
        //ETA保存
        private void Save_ETA()
        {
            m_strOwnshipData[12] = (stackPanel_ETA_Edit.Children[1] as TextBlock).Text + "/" + (stackPanel_ETA_Edit.Children[0] as TextBlock).Text;
            m_strOwnshipData[13] = (stackPanel_ETA_Edit.Children[2] as TextBlock).Text + ":" + (stackPanel_ETA_Edit.Children[3] as TextBlock).Text;
        }
        //航路点界面高亮参数
        int m_iWpChoose = 1;
        //航路点界面高亮
        private void Draw_WayPoint()
        {
            if (m_iWpChoose < 6)
            {
                for (int i = 1; i < 6; i++)
                {

                    (stackPanel_Wp_Num.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_Wp_Num.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Background = Brushes.Transparent;
                (stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Background = Brushes.Black;
            }
                //EXIT
            else if (m_iWpChoose == 6)
            {
                for (int i = 1; i < 6; i++)
                {

                    (stackPanel_Wp_Num.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_Wp_Num.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Foreground=new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock). Foreground= Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Foreground= Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Foreground= Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Foreground= Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Background = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Background = Brushes.Transparent;
           
            }
                //SCORLL
            else if (m_iWpChoose == 7)
            {
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Background = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Background = Brushes.Transparent;
            }
                //SAVE
            else if (m_iWpChoose == 8)
            {
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Background = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Background = Brushes.Transparent;
            }
                //ALL CLEAR
            else if (m_iWpChoose == 9)
            {
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Background = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Background = Brushes.Transparent;
            }
                //REVERSE
            else if (m_iWpChoose == 10)
            {
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[4] as TextBlock).Background = Brushes.Black;
                (Wrappanel_Wp_BottomBox.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_Wp_BottomBox.Children[0] as TextBlock).Background = Brushes.Transparent;
            }
        }
        int m_iWpEditChoose = 0;
        //航路点编辑界面高亮
        private void Draw_WayPointEdit()
        {
            for (int i = 0; i < 17; i++)
            {

                (warppanel_WayPointEdit.Children[i] as TextBlock).Foreground = Brushes.Black;
                (warppanel_WayPointEdit.Children[i] as TextBlock).Background = Brushes.Transparent;

            }
            (warppanel_WayPointEdit.Children[m_iWpEditChoose] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
            (warppanel_WayPointEdit.Children[m_iWpEditChoose] as TextBlock).Background = Brushes.Black;
        }
        //航路点编辑参数
        int m_iLAT_Degree_Ten = 0;
        int m_iLAT_Degree_Unit = 0;
        int m_iLAT_Minute_Ten = 0;
        int m_iLAT_Minute_Unit = 0;
        int m_iLAT_Minute_Tenths = 0;
        int m_iLAT_Minute_Percent = 0;
        int m_iLAT_Minute_Thousands= 0;
        int m_iLON_Degree_Hundred = 0;
        int m_iLON_Degree_Ten = 0;
        int m_iLON_Degree_Unit = 0;
        int m_iLON_Minute_Ten = 0;
        int m_iLON_Minute_Unit = 0;
        int m_iLON_Minute_Tenths = 0;
        int m_iLON_Minute_Percent = 0;
        int m_iLON_Minute_Thousands = 0;
        //航路点保存
        private void WayPointSave()
        {
            if ((warppanel_WayPointEdit.Children[0] as TextBlock).Text == "N")
            {
                m_WpLAT[m_iWpChoose-1] = m_iLAT_Degree_Ten * 600 + m_iLAT_Degree_Unit * 60 + m_iLAT_Minute_Ten * 10 + m_iLAT_Minute_Unit + m_iLAT_Minute_Tenths * 0.1 + m_iLAT_Minute_Percent * 0.01 + m_iLAT_Minute_Thousands * 0.001;
            }
            else
            {
                m_WpLAT[m_iWpChoose-1] = -(m_iLAT_Degree_Ten * 600 + m_iLAT_Degree_Unit * 60 + m_iLAT_Minute_Ten * 10 + m_iLAT_Minute_Unit + m_iLAT_Minute_Tenths * 0.1 + m_iLAT_Minute_Percent * 0.01 + m_iLAT_Minute_Thousands * 0.001);
            }
            if ((warppanel_WayPointEdit.Children[8] as TextBlock).Text == "E")
            {
                m_WpLON[m_iWpChoose-1] = m_iLON_Degree_Hundred * 6000 + m_iLON_Degree_Ten * 600 + m_iLON_Degree_Unit * 60 + m_iLON_Minute_Ten * 10 + m_iLON_Minute_Unit + m_iLON_Minute_Tenths * 0.1 + m_iLON_Minute_Percent * 0.01 + m_iLON_Minute_Thousands * 0.001;
            }
            else
            {
                m_WpLON[m_iWpChoose-1] =-(m_iLON_Degree_Hundred * 6000 + m_iLON_Degree_Ten * 600 + m_iLON_Degree_Unit * 60 + m_iLON_Minute_Ten * 10 + m_iLON_Minute_Unit + m_iLON_Minute_Tenths * 0.1 + m_iLON_Minute_Percent * 0.01 + m_iLON_Minute_Thousands * 0.001);
            }
        }

        //Alarm 设置
        private void Alarm_Setup()
        {
            (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = m_fGuardRNG.ToString("0.0") + "NM";
            (stackPanel_6_Mainmenu.Children[5] as TextBlock).Text = m_fLostRNG.ToString("0.0") + "NM";
        }      
        //保存信息函数
        private void Save_Message()
        {
            m_strMessage[m_iMessage,0]=m_strData[m_iMMSiSel,2];
            m_strMessage[m_iMessage, 1] = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            m_strMessage[m_iMessage, 2] = m_strData[m_iMMSiSel, 3];
            if ((stackPanel_TX.Children[2] as TextBlock).Text == "2.CATAGORY : " + "ROUTINE")
            {
                m_strMessage[m_iMessage, 3] = "CATAGORY:" + "ROUTINE";
            }
            else
            {
                m_strMessage[m_iMessage, 3] = "CATAGORY:" + "SAFETY";
            }
            if ((stackPanel_TX.Children[5] as TextBlock).Text == "4.REPLY : " + "ON")
            {
                m_strMessage[m_iMessage, 4] = "REPLY:" + "ON";
            }
            else
            {
                m_strMessage[m_iMessage, 4] = "REPLY:" + "OFF";
            }
            if ((stackPanel_TX.Children[4] as TextBlock).Text == "TEXT")
            {
                m_strMessage[m_iMessage, 5] = "FUNCTION:" + (stackPanel_TX.Children[4] as TextBlock).Text;
            }
            else
            {
                m_strMessage[m_iMessage, 5] = "FUNCTION:" + "CAPABILITY";
            }
            if ((stackPanel_TX.Children[7] as TextBlock).Text == "5.CH : " + "A/B")
            {
                m_strMessage[m_iMessage, 6] = "CH:A/B";
            }
            else if ((stackPanel_TX.Children[7] as TextBlock).Text == "5.CH : " + "A")
            {
                m_strMessage[m_iMessage, 6] = "CH:A";
            }
            else if ((stackPanel_TX.Children[7] as TextBlock).Text == "5.CH : " + "B")
            {
                m_strMessage[m_iMessage, 6] = "CH:B";
            }
            else 
            {
                m_strMessage[m_iMessage, 6] = "CH:AUTO";
            }
            m_strMessage[m_iMessage, 7] =textBlock_InputText.Text;
            m_iMessage++;
            if (m_iMessage > 10)
            {
                m_iMessage = 0;
            }
        }
        //TX高亮参数
        int m_iTXSel = 0;
        //TX高亮
        private void Draw_TX()
        {
            for (int i = 0; i < 10; i++)
            {

                (stackPanel_TX.Children[i] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_TX.Children[i] as TextBlock).Background = Brushes.Transparent;

            }
            (stackPanel_TX.Children[m_iTXSel] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
            (stackPanel_TX.Children[m_iTXSel] as TextBlock).Background = Brushes.Black;
        }
        //轮询(INTERROGATION)高亮参数
        int m_iIntRogationSel = 0;
        //轮询(INTERROGATION)高亮
        private void Draw_IntRogation()
        {
            if (m_iIntRogationSel < 9)
            {
                for (int i = 0; i < 11; i++)
                {

                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;
                }
                (Wrappanel_InterRogation.Children[0] as TextBlock).Background = Brushes.Transparent; ;
                (Wrappanel_InterRogation.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[1] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Foreground = Brushes.Black;
                (stackPanel_6_Mainmenu.Children[m_iIntRogationSel] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_6_Mainmenu.Children[m_iIntRogationSel] as TextBlock).Background = Brushes.Black;

            }
                //EXIT
            else if (m_iIntRogationSel == 10)
            {
                for (int i = 0; i < 11; i++)
                {
                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_6_Mainmenu.Children[i] as TextBlock).Background = Brushes.Transparent;
                }
                (Wrappanel_InterRogation.Children[0] as TextBlock).Background = Brushes.Black;
                (Wrappanel_InterRogation.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_InterRogation.Children[1] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Foreground = Brushes.Black;
            }
                //TX
            else if (m_iIntRogationSel == 12)
            {
                (Wrappanel_InterRogation.Children[1] as TextBlock).Background = Brushes.Black;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[1] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_InterRogation.Children[0] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Foreground = Brushes.Black;
            }
                //CLEAR
            else if (m_iIntRogationSel == 14)
            {
                (Wrappanel_InterRogation.Children[2] as TextBlock).Background = Brushes.Black;
                (Wrappanel_InterRogation.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_InterRogation.Children[1] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Foreground = Brushes.Black;
            }
                //CHK1-1
            else if (m_iIntRogationSel == 16)
            {
                (Wrappanel_InterRogation.Children[3] as TextBlock).Background = Brushes.Black;
                (Wrappanel_InterRogation.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_InterRogation.Children[1] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Foreground = Brushes.Black;
            }
                //CHK1-2
            else if (m_iIntRogationSel == 18)
            {
                (Wrappanel_InterRogation.Children[4] as TextBlock).Background = Brushes.Black;
                (Wrappanel_InterRogation.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_InterRogation.Children[1] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Foreground = Brushes.Black;
            }
                //CHK2
            else if (m_iIntRogationSel == 20)
            {
                (Wrappanel_InterRogation.Children[5] as TextBlock).Background = Brushes.Black;
                (Wrappanel_InterRogation.Children[1] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Background = Brushes.Transparent;
                (Wrappanel_InterRogation.Children[5] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (Wrappanel_InterRogation.Children[1] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[2] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[3] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[4] as TextBlock).Foreground = Brushes.Black;
                (Wrappanel_InterRogation.Children[0] as TextBlock).Foreground = Brushes.Black;
            }
        }
        //发件箱高亮参数
        int m_iTXSend_Sel = 0;
        //收发件箱高亮及数据显示
        private void TX_Send_Receive_Disp()
        {
  
            if (m_iFunOrder == 611)
            {
                for (int i = 0; i < 10; i++)
                {

                    (stackPanel_TX_Send.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_TX_Send.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                (stackPanel_TX_Send.Children[m_iTXSend_Sel] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_TX_Send.Children[m_iTXSend_Sel] as TextBlock).Background = Brushes.Black;
                (warppanel_TX_Send.Children[0] as TextBlock).Text = m_strMessage[m_iTXSend_Sel, 1];
                (warppanel_TX_Send.Children[1] as TextBlock).Text = m_strMessage[m_iTXSend_Sel, 2];
                (warppanel_TX_Send.Children[2] as TextBlock).Text = m_strMessage[m_iTXSend_Sel, 3];
                (warppanel_TX_Send.Children[3] as TextBlock).Text = m_strMessage[m_iTXSend_Sel, 4];
                (warppanel_TX_Send.Children[4] as TextBlock).Text = m_strMessage[m_iTXSend_Sel, 5];
                (warppanel_TX_Send.Children[5] as TextBlock).Text = m_strMessage[m_iTXSend_Sel, 6];
            }
            else if (m_iFunOrder == 612)
            {
                for (int i = 0; i < 10; i++)
                {

                    (stackPanel_TX_Send.Children[i] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_TX_Send.Children[i] as TextBlock).Background = Brushes.Transparent;

                }
                (stackPanel_TX_Send.Children[m_iTXSend_Sel] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                (stackPanel_TX_Send.Children[m_iTXSend_Sel] as TextBlock).Background = Brushes.Black;
                (warppanel_TX_Send.Children[0] as TextBlock).Text = m_strMessageReceive[m_iTXSend_Sel, 1];
                (warppanel_TX_Send.Children[1] as TextBlock).Text = m_strMessageReceive[m_iTXSend_Sel, 2];
                (warppanel_TX_Send.Children[2] as TextBlock).Text = m_strMessageReceive[m_iTXSend_Sel, 3];
                (warppanel_TX_Send.Children[3] as TextBlock).Text = m_strMessageReceive[m_iTXSend_Sel, 4];
                (warppanel_TX_Send.Children[4] as TextBlock).Text = m_strMessageReceive[m_iTXSend_Sel, 5];
                (warppanel_TX_Send.Children[5] as TextBlock).Text = m_strMessageReceive[m_iTXSend_Sel, 6];
            }
           
        }

        private void Button_MouseLeftButtonDown_MENU(object sender, MouseButtonEventArgs e)//MENU键的左键按下事件,1
        {
            Button_Sound();
            Draw_My_Button(0, 1);
            if (m_iFunOrder != 0)
            {
                m_iFunOrder = 6;
                Draw_My_Screen(m_Struct_No1);
                Data_Display();
                TitleDisplay(10);
                MainMenu_Change();
                Draw_MainMenu();

            }

        }

        private void Button_MouseLeftButtonUp_MENU(object sender, MouseButtonEventArgs e)//MENU键的左键抬起事件,1
        {
            Draw_My_Button(1, 1);//绘制按钮抬起效果
        }

        private void Button_MouseLeftButtonDown_CLR(object sender, MouseButtonEventArgs e)//CLR键的左键按下事件,2
        {
            Button_Sound();
            m_Warnsound = false;
            Warn_Sound();          
            Draw_My_Button(0, 2);//绘制按钮按下效果
            if (m_iFunOrder == 2)
            {
                if (rectangle_ChoseSign_Top.Visibility == Visibility.Visible)
                {
                    (stackPanel_2_Stakpnl_Setup.Children[1] as TextBlock).Background = Brushes.Black;
                    (stackPanel_2_Stakpnl_Setup.Children[1] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                }
                else
                {
                    (stackPanel_2_Stakpnl_Setup.Children[1] as TextBlock).Background = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_2_Stakpnl_Setup.Children[1] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_2_3_Left.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Right.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Mid.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Guard.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Left.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_2_3_Right.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_2_3_Mid.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_2_3_Guard.Children[0] as TextBlock).Foreground = Brushes.Black;
                    rectangle_ChoseSign_Top.Visibility = Visibility.Visible;
                }
            }
            else if (m_iFunOrder == 3)
            {
                m_Detail_Choose = 0;
                m_iFunOrder = 1;
                BottomDisplay();
                TitleDisplay(m_iTitle_Choose);
                Draw_My_Screen(m_Struct_No1);//绘制界面
                Data_Display();
            }
            else if (m_iFunOrder == 4)
            {
                Draw_Choose(0);
                m_iFunOrder = 1;
                Draw_My_Screen(m_Struct_No1);//绘制界面
                Data_Display();
             

            }
            else if (m_iFunOrder == 420)
            {

                m_iFunOrder = 4;
                m_iSelect = 0;
                Draw_My_Screen(m_Struct_No1);
                Data_Display();
                Draw_Select();
            }
            else if (m_iFunOrder == 43)
            {
                m_iFunOrder = 4;
                m_iSelect = 0;
                m_Detail_Choose = 0;
                Draw_My_Screen(m_Struct_No1);
                Data_Display();
                Draw_Select();
                TitleDisplay(m_iTitle_Choose);

            }
            else if (m_iFunOrder == 41)
            {
                m_iFunOrder = 4;
                m_iSelect = 0;
                grid_4_Down_List.Visibility = Visibility.Collapsed;


                warpPanel_4_Down.Visibility = Visibility.Visible;
                grid_4_Down.Visibility = Visibility.Visible;
                
                Data_Display();
                Draw_Select();
                TitleDisplay(m_iTitle_Choose);
            }
            else if (m_iFunOrder == 411 || m_iFunOrder == 412 || m_iFunOrder == 413)
            {
                m_iFunOrder = 41;
                m_iList_Select = 0;
                Draw_Select();
            }
            else if (m_iFunOrder == 5)
            {
                m_bPower = false;
                if (m_iPassword_sel > 0)
                {
                    m_iPassword_sel--;
                    Draw_Password();
            
                }
                if (m_strPassCodeinPut.Length > 0)
                {
                    m_strPassCodeinPut = m_strPassCodeinPut.Substring(0, m_strPassCodeinPut.Length - 1);
                }
               
            }
            else if (m_iFunOrder == 6)
            {
                m_iFunOrder = 1;
                Draw_My_Screen(m_Struct_No1);
                Data_Display();
                Draw_Choose(0);
                m_Count_Choose = 0;     
               
             
             
                TitleDisplay(m_iTitle_Choose);

            }
            else if(m_iFunOrder>59&&m_iFunOrder<65)
            {
                 m_iFunOrder=6;
                  MainMenu_Change();

                Draw_MainMenu();             
            }
            else if (m_iFunOrder ==7 )
            {
                if (textBlock_InputText.Text.Length > 0)
                {
                    textBlock_InputText.Text = textBlock_InputText.Text.Substring(0, textBlock_InputText.Text.Length - 1);
                }
            }
            else if (m_iFunOrder >609&&m_iFunOrder<615)
            {
                Grid_6.Visibility = Visibility.Visible;
                m_iFunOrder = 61;
                MainMenu_Change();
                Draw_MainMenu();
            }
            else if (m_iFunOrder == 606)
            {
                m_iFunOrder = 60;
                MainMenu_Change();
                Draw_MainMenu();
            }
            else if (m_iFunOrder == 603)
            {
                m_iFunOrder = 60;
                MainMenu_Change();
                Draw_MainMenu();
            }
            else if (m_iFunOrder == 6060)
            {
                m_iFunOrder = 606;
                MainMenu_Change();
                Draw_WayPoint();
            }
            else if (m_iFunOrder == 6111)
            {
                m_iFunOrder = 611;
                MainMenu_Change();
                TX_Send_Receive_Disp();
            }
            else if (m_iFunOrder == 6121)
            {
                m_iFunOrder = 612;
                MainMenu_Change();
                TX_Send_Receive_Disp();
            }

            else if (m_iFunOrder == 622)
            {
                m_iFunOrder = 62;
                MainMenu_Change();
                Draw_MainMenu();
            }
            else if (m_iFunOrder > 639 && m_iFunOrder < 646)
            {
                m_iFunOrder = 64;
                MainMenu_Change();
                Draw_MainMenu();

            }

        }

        private void Button_MouseLeftButtonUp_CLR(object sender, MouseButtonEventArgs e)//CLR键的左键抬起事件,2
        {
            Draw_My_Button(1, 2);//绘制按钮抬起效果
        }

        private void Button_MouseLeftButtonDown_DSPL(object sender, MouseButtonEventArgs e)//DSPL键的左键按下事件,3
        {
            Button_Sound();
            Draw_My_Button(0, 3);//绘制按钮按下效果
            if (m_iFunOrder != 0 && m_iFunOrder != 2)
            {
                if (m_iFunOrder == 20)
                {
                    //所有参数恢复默认值
                    m_fDispRNG = 1.5f;
                    m_iBRG_Disp = 0;
                    m_iSORT_Disp = 0;
                    m_fGuardRNG = 12;
                    m_iNumOfShip = 22;
                    m_iContrast = 7;
                    m_bAutoRNG = false;
                    m_iFunOrder = 2;
                }
                m_Count_Choose = 0;
                Draw_Choose(0);                     
                m_iFunOrder = 2;
                Draw_My_Screen(m_Struct_No1);//绘制界面
                Data_Display();
                m_DispChoose = 0;
                m_iGrphic_Sel = 0;
                RealTime_Disp();
                


             
             
            }
             else if (m_iFunOrder == 2)
            {
                m_Count_Choose = 0;          
                Draw_Choose(0);
                m_iFunOrder = 1;
                m_DispChoose = 0;
                Draw_My_Screen(m_Struct_No1);//绘制界面
                Data_Display();
               
            }
        }

        private void Button_MouseLeftButtonUp_DSPL(object sender, MouseButtonEventArgs e)//DSPL键的左键抬起事件,3
        {
            Draw_My_Button(1, 3);//绘制按钮抬起效果
        }

        private void Button_MouseLeftButtonDown_PWR(object sender, MouseButtonEventArgs e)//PWR键的左键按下事件，4
        {

                Button_Sound();
                Cal_Time_Powon();
                Draw_My_Button(0, 4);//绘制按钮按下效果
                        
            
     
        }

        private void Button_MouseLeftButtonUp_PWR(object sender, MouseButtonEventArgs e)//PWR键的左键抬起事件，4
        {

            Draw_My_Button(1, 4);//绘制按钮抬起效果
            m_PowOn.Stop();
            if (m_bIsShowDoubleHand == true)
            {
                if (m_iFunOrder == 0 && image_HandOFF.Visibility == Visibility.Collapsed)
                {
                    image4.Visibility = Visibility.Visible;
                   
                    image_HandPow.Visibility = Visibility.Visible;
                }
                else if (m_iFunOrder != 0 && image_HandOFF.Visibility == Visibility.Collapsed)
                {
                    image4_ON.Visibility = Visibility.Visible;

                    image_HandPow.Visibility = Visibility.Visible;
                }
        
            }
           
        }

        private void Button_MouseLeftButtonDown_OFF(object sender, MouseButtonEventArgs e)//OFF键的左键按下事件，5
        {
            Button_Sound();
            if (m_iFunOrder == 1)
            {
                m_iFunOrder = 5;
                Draw_My_Screen(m_Struct_No1);
                m_iPassword_sel = 0;
                Draw_Password();
                Data_Display();
                m_iKeyBoard_Sel = 0;
                Sel_KeyBoard();
                
            }
            Cal_Time_Powon();
                   
            
              Draw_My_Button(0, 5);//绘制按钮按下效果
        }

        private void Button_MouseLeftButtonUp_OFF(object sender, MouseButtonEventArgs e)//OFF键的左键抬起事件，5
        {

            Draw_My_Button(1, 5);//绘制按钮抬起效果
            if (m_bIsShowDoubleHand == true)
            {
                if (m_iFunOrder == 0 && image_HandPow.Visibility == Visibility.Collapsed)
                {
                    image5.Visibility = Visibility.Visible;
                    image_HandOFF.Visibility = Visibility.Visible;

                }
                else if (m_iFunOrder != 0 && image_HandPow.Visibility == Visibility.Collapsed)
                {
                    image5_ON.Visibility = Visibility.Visible;
                    image_HandOFF.Visibility = Visibility.Visible;
                }

                m_PowOn.Stop();
            }

          
        }

        private void Button_MouseLeftButtonDown_Stk_up(object sender, MouseButtonEventArgs e)//摇杆上键的左键按下事件，6
        {
            if (m_iFunOrder == 1)
            {
                if (m_Count_Choose == 0 && rectangle_ChoseSign_Top.Visibility == Visibility.Collapsed && m_Count_Choose_Up == 0)
                {
                    Draw_Choose(0);
                }
                else if (rectangle_ChoseSign_Top.Visibility == Visibility.Visible)
                {
                    TitleDisplay(m_iTitle_Choose);
                    m_iFunOrder = 4;
                    m_iSelect = 0;
                    Draw_My_Screen(m_Struct_No1);
                    Data_Display();
                    Draw_Select();
                }
                Roll_Data(1);
               

            }
                       
            else if (m_iFunOrder == 2 )//绘制高亮，DSPL界面
            {
                if (m_DispChoose == 0)
                {
                    rectangle_ChoseSign_Top.Visibility = Visibility.Visible;
                    (stackPanel_2_3_Left.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Right.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Mid.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Guard.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Left.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_2_3_Right.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_2_3_Mid.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_2_3_Guard.Children[0] as TextBlock).Foreground = Brushes.Black;

                }
                if (m_DispChoose > 0)
                {
                    m_DispChoose--;
                    Disp_ShipChoose(m_DispChoose);
                }
                
       

            }
            else if (m_iFunOrder == 20)
            {
                if (m_iGrphic_Sel % 2 == 0 && m_iGrphic_Sel >0)
                {
                    m_iGrphic_Sel -= 2;
                    Draw_Grphic_Setup();
                }
            }
            else if (m_iFunOrder == 3&&m_Detail_Choose>0)
            {    
               
                m_Detail_Choose--;
                Data_Display();


            }
            else if (m_iFunOrder == 4)//绘制高亮，主界面菜单
            {
                if (m_iSelect > 1 && m_iSelect < 6)
                {
                    m_iSelect -= 2;
                    Draw_Select();
                }

            }
            else if (m_iFunOrder == 41 && m_iList_Select > 0)//List菜单高亮
            {
                m_iList_Select--;
                Draw_Select();
            } 
            else if (m_iFunOrder == 43 && m_Detail_Choose > 0)//数据翻页，本船详细信息界面
            {
                m_Detail_Choose--;
                Data_Display();
            }
            else if (m_iFunOrder == 5 && m_iKeyBoard_Sel > 16)
            {
                m_iKeyBoard_Sel -= 17;
                Sel_KeyBoard();
            }
            else if (m_iFunOrder == 7&& m_iKeyBoard_Sel > 16)
            {
                m_iKeyBoard_Sel -= 17;
                Sel_KeyBoard();
            }
            else if (m_iFunOrder == 70&& m_iKeyBoard_Sel > 16)
            {
                m_iKeyBoard_Sel -= 17;
                Sel_KeyBoard();
            }
            else if (m_iFunOrder == 6)
            {
                if (m_iMainMenu_Sel >1)
                {
                    m_iMainMenu_Sel -= 2;
                    Draw_MainMenu();
                }
            }
            else if (m_iMainMenu_60 % 2 == 0&&m_iFunOrder == 60)
            {
                if (m_iMainMenu_60 > 1&&m_iMainMenu_60<9)
                {
                    m_iMainMenu_60 -= 2;          
                    Draw_MainMenu();
                }         
                else if (m_iMainMenu_60 > 8)
                    {
                        m_iMainMenu_60 -= 2; 
                        if (m_iMainMenu_60 < 9)
                        {
                            MainMenu_Change();                          
                        }
                        Draw_MainMenu();
                    }      
            }
            else if (m_iMainMenu_60 == 7)
            {
                m_fOwnDraught-=0.1f;
                if (m_fOwnDraught < 0)
                {
                    m_fOwnDraught = 25;
                }
                m_strOwnshipData[16] = m_fOwnDraught.ToString("0.0");
                (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = m_strOwnshipData[16] + "M";
            }
            else if (m_iFunOrder == 61)
            {
                if (m_iMainMenu_61 >1)
                {
                    m_iMainMenu_61 -= 2;
                    Draw_MainMenu();
                }
           
            }
            else if (m_iFunOrder == 606)
            {
                if (m_iWpChoose > 1)
                {
                    m_iWpChoose--;
                    Draw_WayPoint();
                }
            }
            else if (m_iFunOrder == 6060)
            {
                //经纬度属性设置
                if (m_iWpEditChoose == 0)
                {
                    if ((warppanel_WayPointEdit.Children[0] as TextBlock).Text == "N")
                    {
                        (warppanel_WayPointEdit.Children[0] as TextBlock).Text = "S";
                    }
                    else
                    {
                        (warppanel_WayPointEdit.Children[0] as TextBlock).Text = "N";
                    }
                }
                else if (m_iWpEditChoose == 8)
                {
                    if ((warppanel_WayPointEdit.Children[8] as TextBlock).Text == "E")
                    {
                        (warppanel_WayPointEdit.Children[8] as TextBlock).Text = "W";
                    }
                    else
                    {
                        (warppanel_WayPointEdit.Children[8] as TextBlock).Text = "E";
                    }
                }
                else if (m_iWpEditChoose == 1)
                {
                    m_iLAT_Degree_Ten--;
                    if (m_iLAT_Degree_Ten < 0)
                    {
                        m_iLAT_Degree_Ten = 9;
                    }
                    (warppanel_WayPointEdit.Children[1] as TextBlock).Text = m_iLAT_Degree_Ten.ToString();
                }
                else if (m_iWpEditChoose == 2)
                {
                    m_iLAT_Degree_Unit--;
                    if (m_iLAT_Degree_Unit < 0)
                    {
                        m_iLAT_Degree_Unit = 9;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Degree_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[2] as TextBlock).Text = m_iLAT_Degree_Unit.ToString();
                }
                else if (m_iWpEditChoose == 3)
                {
                    m_iLAT_Minute_Ten--;
                    if (m_iLAT_Minute_Ten < 0)
                    {
                        m_iLAT_Minute_Ten = 6;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Ten = 0;
                    }
                    (warppanel_WayPointEdit.Children[3] as TextBlock).Text = m_iLAT_Minute_Ten.ToString();
                }
                else if (m_iWpEditChoose == 4)
                {
                    m_iLAT_Minute_Unit--;
                    if (m_iLAT_Minute_Unit < 0)
                    {
                        m_iLAT_Minute_Unit = 9;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[4] as TextBlock).Text = m_iLAT_Minute_Unit.ToString();
                }
                else if (m_iWpEditChoose == 5)
                {
                    m_iLAT_Minute_Tenths--;
                    if (m_iLAT_Minute_Tenths < 0)
                    {
                        m_iLAT_Minute_Tenths = 9;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Tenths = 0;
                    }
                    (warppanel_WayPointEdit.Children[5] as TextBlock).Text = m_iLAT_Minute_Tenths.ToString();
                }
                else if (m_iWpEditChoose == 6)
                {
                    m_iLAT_Minute_Percent--;
                    if (m_iLAT_Minute_Percent < 0)
                    {
                        m_iLAT_Minute_Percent = 9;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Percent = 0;
                    }
                    (warppanel_WayPointEdit.Children[6] as TextBlock).Text = m_iLAT_Minute_Percent.ToString();
                }
                else if (m_iWpEditChoose == 7)
                {
                    m_iLAT_Minute_Thousands--;
                    if (m_iLAT_Minute_Thousands < 0)
                    {
                        m_iLAT_Minute_Thousands = 9;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Thousands = 0;
                    }
                    (warppanel_WayPointEdit.Children[7] as TextBlock).Text = m_iLAT_Minute_Thousands.ToString();
                }
                else if (m_iWpEditChoose == 9)
                {
                    m_iLON_Degree_Hundred--;
                    if (m_iLON_Degree_Hundred < 0)
                    {
                        m_iLON_Degree_Hundred = 1;
                    }
                    (warppanel_WayPointEdit.Children[9] as TextBlock).Text = m_iLON_Degree_Hundred.ToString();
                }
                else if (m_iWpEditChoose == 10)
                {
                    m_iLON_Degree_Ten--;
                    if (m_iLON_Degree_Ten < 0)
                    {
                        m_iLON_Degree_Ten = 9;
                        if (m_iLON_Degree_Hundred == 1)
                        {
                            m_iLON_Degree_Ten = 8;
                        }
                    }


                    (warppanel_WayPointEdit.Children[10] as TextBlock).Text = m_iLON_Degree_Ten.ToString();
                }
                else if (m_iWpEditChoose == 11)
                {
                    m_iLON_Degree_Unit--;
                    if (m_iLON_Degree_Unit < 0)
                    {
                        m_iLON_Degree_Unit = 9;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Degree_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[11] as TextBlock).Text = m_iLON_Degree_Unit.ToString();
                }
                else if (m_iWpEditChoose == 12)
                {
                    m_iLON_Minute_Ten--;
                    if (m_iLON_Minute_Ten < 0)
                    {
                        m_iLON_Minute_Ten = 6;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Ten = 0;
                    }
                    (warppanel_WayPointEdit.Children[12] as TextBlock).Text = m_iLON_Minute_Ten.ToString();
                }
                else if (m_iWpEditChoose == 13)
                {
                    m_iLON_Minute_Unit--;
                    if (m_iLON_Minute_Unit < 0)
                    {
                        m_iLON_Minute_Unit = 9;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[13] as TextBlock).Text = m_iLON_Minute_Unit.ToString();
                }
                else if (m_iWpEditChoose == 14)
                {
                    m_iLON_Minute_Tenths--;
                    if (m_iLON_Minute_Tenths < 0)
                    {
                        m_iLON_Minute_Tenths = 9;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Tenths = 0;
                    }
                    (warppanel_WayPointEdit.Children[14] as TextBlock).Text = m_iLON_Minute_Tenths.ToString();
                }
                else if (m_iWpEditChoose == 15)
                {
                    m_iLON_Minute_Percent--;
                    if (m_iLON_Minute_Percent < 0)
                    {
                        m_iLON_Minute_Percent = 9;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Percent = 0;
                    }
                    (warppanel_WayPointEdit.Children[15] as TextBlock).Text = m_iLON_Minute_Percent.ToString();
                }
                else if (m_iWpEditChoose == 16)
                {
                    m_iLON_Minute_Thousands--;
                    if (m_iLON_Minute_Thousands < 0)
                    {
                        m_iLON_Minute_Thousands = 9;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Thousands = 0;
                    }
                    (warppanel_WayPointEdit.Children[16] as TextBlock).Text = m_iLON_Minute_Thousands.ToString();
                }
            }
            
            else if (m_iFunOrder == 610)
            {
                if (m_iTXSel > 0&&m_iTXSel<4)
                {
                    m_iTXSel--;
                    Draw_TX();
                }
                else if (m_iTXSel > 4 && m_iTXSel < 10)
                {
                    m_iTXSel -= 2;
                    Draw_TX();
                }

            }
            else if (m_iFunOrder == 611 || m_iFunOrder == 612)
            {
                if (m_iTXSend_Sel > 0)
                {
                    m_iTXSend_Sel--;
                }
                TX_Send_Receive_Disp();
            }
            else if (m_iFunOrder == 613)
            {
                if (m_iIntRogationSel > 0)
                {
                    m_iIntRogationSel -= 2;
                    Draw_IntRogation();
                }
            }
            else if (m_iFunOrder == 62 && m_iMainMenu_62 % 3 == 1)
            {
                if (m_iMainMenu_62 >1)
                {
                    m_iMainMenu_62 -= 3;
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 63 && m_iMainMenu_63 % 2 == 0)
            {
                if (m_iMainMenu_63 > 1 && m_iMainMenu_63 <9)
                {
                    m_iMainMenu_63 -= 2;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_63 > 8)
                {
                    m_iMainMenu_63 -= 2;
                    if (m_iMainMenu_63 < 9)
                    {
                        MainMenu_Change();
                    }
                    Draw_MainMenu();
                }      
            }
            else if (m_iFunOrder == 64)
            {
                if (m_iMainMenu_64 >1)
                {
                    m_iMainMenu_64 -= 2;
                    Draw_MainMenu();
                }
            }
        
        
            Draw_My_Button(0, 6);//绘制按钮按下效果
        }

        private void Button_MouseLeftButtonUp_Stk_up(object sender, MouseButtonEventArgs e)//摇杆上键的左键抬起事件，6
        {
            Draw_My_Button(1, 6);//绘制按钮抬起效果
        }

        private void Button_MouseLeftButtonDown_Stk_down(object sender, MouseButtonEventArgs e)//摇杆下键的左键按下事件,7
        {
            polygon_ChoseSign_Down.Fill = Brushes.Black;
            if (m_iFunOrder == 1)
            {
                Roll_Data(2);
            }
            else if (m_iFunOrder == 2 && m_DispChoose < m_iTotal-3)
            {
                if (rectangle_ChoseSign_Top.Visibility == Visibility.Visible)
                {
                    rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                    (stackPanel_2_3_Left.Children[0] as TextBlock).Background = Brushes.Black;
                    (stackPanel_2_3_Right.Children[0] as TextBlock).Background = Brushes.Black;
                    (stackPanel_2_3_Mid.Children[0] as TextBlock).Background = Brushes.Black;
                    (stackPanel_2_3_Guard.Children[0] as TextBlock).Background = Brushes.Black;
                    (stackPanel_2_3_Left.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_2_3_Right.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_2_3_Mid.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                    (stackPanel_2_3_Guard.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;

                }
                else
                {
                    m_DispChoose++;
                    Disp_ShipChoose(m_DispChoose);
                }
                
                

            }
            else if (m_iFunOrder == 20)
            {
                if (m_iGrphic_Sel % 2 == 0&&m_iGrphic_Sel<15)
                {
                    m_iGrphic_Sel += 2;
                    Draw_Grphic_Setup();
                }
            }
            else if (m_iFunOrder == 3 && m_Detail_Choose < 4)//数据翻页，船舶详细信息界面
            {
                m_Detail_Choose++;
                Data_Display();

            }
            else if (m_iFunOrder == 4)//绘制高亮，主界面菜单
            {
                if (m_iSelect > -1 && m_iSelect < 4)
                {
                    m_iSelect += 2;
                    Draw_Select();
                }
            }
            else if (m_iFunOrder == 41 && m_iList_Select < 2)//List菜单高亮
            {
                m_iList_Select++;
                Draw_Select();
            }

            else if (m_iFunOrder == 43 && m_Detail_Choose < 1)//数据翻页，本船详细信息界面
            {
                m_Detail_Choose++;
                Data_Display();

            }
            else if (m_iFunOrder == 5 && m_iKeyBoard_Sel < 49)
            {
                m_iKeyBoard_Sel += 17;
                Sel_KeyBoard();
            }
            else if (m_iFunOrder == 7 && m_iKeyBoard_Sel < 49)
            {
                m_iKeyBoard_Sel += 17;
                Sel_KeyBoard();
            }
            else if (m_iFunOrder == 70 && m_iKeyBoard_Sel < 49)
            {
                m_iKeyBoard_Sel += 17;
                Sel_KeyBoard();
            }
            else if (m_iFunOrder == 6)
            {
                if (m_iMainMenu_Sel<9)
                {
                    m_iMainMenu_Sel += 2;
                    Draw_MainMenu();
                }
            }
            else if (m_iMainMenu_60 % 2 == 0&&m_iFunOrder == 60)
            {
                if (m_iMainMenu_60 < 9)
                {
                    m_iMainMenu_60 += 2;
                    
                    if (m_iMainMenu_60 > 8)
                    {
                        MainMenu_Change();
                    }
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 > 8&&m_iMainMenu_60<15)
                {
                    m_iMainMenu_60 += 2;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 >15&&m_iMainMenu_60<21)
                {
                    m_iMainMenu_60 += 2;
                    Draw_MainMenu();
                }
                
            }
            else if (m_iMainMenu_60 == 7)
            {
                m_fOwnDraught += 0.1f;
                if (m_fOwnDraught < 0)
                {
                    m_fOwnDraught = 25;
                }
                m_strOwnshipData[16] = m_fOwnDraught.ToString("0.0");
                (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = m_strOwnshipData[16] + "M";
            }
            else if (m_iFunOrder == 61)
            {
                if (m_iMainMenu_61 < 8)
                {
                    m_iMainMenu_61 += 2;
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 606)
            {
                if (m_iWpChoose < 5)
                {
                    m_iWpChoose++;
                    Draw_WayPoint();
                }
                else if (m_iWpChoose < 10)
                {
                    m_iWpChoose++;
                    Draw_WayPoint();
                }
            }
               else if (m_iFunOrder == 6060)
            {
                //经纬度属性设置
                if (m_iWpEditChoose == 0)
                {
                    if ((warppanel_WayPointEdit.Children[0] as TextBlock).Text == "N")
                    {
                        (warppanel_WayPointEdit.Children[0] as TextBlock).Text = "S";
                    }
                    else
                    {
                        (warppanel_WayPointEdit.Children[0] as TextBlock).Text = "N";
                    }
                }
                else if (m_iWpEditChoose == 8)
                {
                    if ((warppanel_WayPointEdit.Children[8] as TextBlock).Text == "E")
                    {
                        (warppanel_WayPointEdit.Children[8] as TextBlock).Text = "W";
                    }
                    else
                    {
                        (warppanel_WayPointEdit.Children[8] as TextBlock).Text = "E";
                    }
                }
                else if (m_iWpEditChoose == 1)
                {
                    m_iLAT_Degree_Ten++;
                    if (m_iLAT_Degree_Ten > 9)
                    {
                        m_iLAT_Degree_Ten = 0;
                    }
                    (warppanel_WayPointEdit.Children[1] as TextBlock).Text = m_iLAT_Degree_Ten.ToString(); 
                }
                else if (m_iWpEditChoose == 2)
                {
                    m_iLAT_Degree_Unit++;
                    if (m_iLAT_Degree_Unit > 9)
                    {
                        m_iLAT_Degree_Unit = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Degree_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[2] as TextBlock).Text = m_iLAT_Degree_Unit.ToString();
                }
                else if (m_iWpEditChoose == 3)
                {
                    m_iLAT_Minute_Ten++;
                    if (m_iLAT_Minute_Ten > 6)
                    {
                        m_iLAT_Minute_Ten = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Ten = 0;
                    }
                    (warppanel_WayPointEdit.Children[3] as TextBlock).Text = m_iLAT_Minute_Ten.ToString();
                }
                else if (m_iWpEditChoose == 4)
                {
                    m_iLAT_Minute_Unit++;
                    if (m_iLAT_Minute_Unit > 9)
                    {
                        m_iLAT_Minute_Unit = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[4] as TextBlock).Text = m_iLAT_Minute_Unit.ToString();
                }
                else if (m_iWpEditChoose == 5)
                {
                    m_iLAT_Minute_Tenths++;
                    if (m_iLAT_Minute_Tenths > 9)
                    {
                        m_iLAT_Minute_Tenths = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Tenths = 0;
                    }
                    (warppanel_WayPointEdit.Children[5] as TextBlock).Text = m_iLAT_Minute_Tenths.ToString();
                }
                else if (m_iWpEditChoose == 6)
                {
                    m_iLAT_Minute_Percent++;
                    if (m_iLAT_Minute_Percent > 9)
                    {
                        m_iLAT_Minute_Percent = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Percent = 0;
                    }
                    (warppanel_WayPointEdit.Children[6] as TextBlock).Text = m_iLAT_Minute_Percent.ToString();
                }
                else if (m_iWpEditChoose == 7)
                {
                    m_iLAT_Minute_Thousands++;
                    if (m_iLAT_Minute_Thousands > 9)
                    {
                        m_iLAT_Minute_Thousands = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Thousands = 0;
                    }
                    (warppanel_WayPointEdit.Children[7] as TextBlock).Text = m_iLAT_Minute_Thousands.ToString();
                }
                else if (m_iWpEditChoose == 9)
                {
                    m_iLON_Degree_Hundred++;
                    if (m_iLON_Degree_Hundred > 1)
                    {
                        m_iLON_Degree_Hundred = 0;
                    }
                    (warppanel_WayPointEdit.Children[9] as TextBlock).Text = m_iLON_Degree_Hundred.ToString();
                }
                else if (m_iWpEditChoose == 10)
                {
                    m_iLON_Degree_Ten++;
                    if (m_iLON_Degree_Hundred == 1)
                    {
                        if (m_iLON_Degree_Ten > 8)
                        {
                            m_iLON_Degree_Ten = 0;
                        }
                    }
                    else if (m_iLON_Degree_Ten > 9)
                    {
                        m_iLON_Degree_Ten = 0;
                    }
                    
                    (warppanel_WayPointEdit.Children[10] as TextBlock).Text = m_iLON_Degree_Ten.ToString();
                }
                else if (m_iWpEditChoose == 11)
                {
                    m_iLON_Degree_Unit++;
                    if (m_iLON_Degree_Unit > 9)
                    {
                        m_iLON_Degree_Unit = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Degree_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[11] as TextBlock).Text = m_iLON_Degree_Unit.ToString();
                }
                else if (m_iWpEditChoose == 12)
                {
                    m_iLON_Minute_Ten++;
                    if (m_iLON_Minute_Ten > 6)
                    {
                        m_iLON_Minute_Ten = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Ten = 0;
                    }
                    (warppanel_WayPointEdit.Children[12] as TextBlock).Text = m_iLON_Minute_Ten.ToString();
                }
                else if (m_iWpEditChoose == 13)
                {
                    m_iLON_Minute_Unit++;
                    if (m_iLON_Minute_Unit > 9)
                    {
                        m_iLON_Minute_Unit = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[13] as TextBlock).Text = m_iLON_Minute_Unit.ToString();
                }
                else if (m_iWpEditChoose == 14)
                {
                    m_iLON_Minute_Tenths++;
                    if (m_iLON_Minute_Tenths > 9)
                    {
                        m_iLON_Minute_Tenths = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Tenths = 0;
                    }
                    (warppanel_WayPointEdit.Children[14] as TextBlock).Text = m_iLON_Minute_Tenths.ToString();
                }
                else if (m_iWpEditChoose == 15)
                {
                    m_iLON_Minute_Percent++;
                    if (m_iLON_Minute_Percent > 9)
                    {
                        m_iLON_Minute_Percent = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Percent = 0;
                    }
                    (warppanel_WayPointEdit.Children[15] as TextBlock).Text = m_iLON_Minute_Percent.ToString();
                }
                else if (m_iWpEditChoose == 16)
                {
                    m_iLON_Minute_Thousands++;
                    if (m_iLON_Minute_Thousands > 9)
                    {
                        m_iLON_Minute_Thousands = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Thousands = 0;
                    }
                    (warppanel_WayPointEdit.Children[16] as TextBlock).Text = m_iLON_Minute_Thousands.ToString();
                }
            }
            else if (m_iFunOrder == 610)
            {
                if (m_iTXSel < 3)
                {
                    m_iTXSel++;
                    Draw_TX();
                }
                else if (m_iTXSel < 8 && m_iTXSel != 4)
                {
                    m_iTXSel += 2;
                    Draw_TX();
                }
                else
                {
                    m_iFunOrder = 70;
                    Draw_My_Screen(m_Struct_No1);
                    Data_Display();
                    m_iKeyBoard_Sel = 0;
                    Sel_KeyBoard();
                }

            }
            else if (m_iFunOrder == 611 || m_iFunOrder == 612)
            {
                if (m_iTXSend_Sel < 9)
                {
                    m_iTXSend_Sel++;
                }
                TX_Send_Receive_Disp();
            }
            else if (m_iFunOrder == 613)
            {
                if (m_iIntRogationSel < 19)
                {
                    m_iIntRogationSel += 2;
                    Draw_IntRogation();
                }
            }
            else if (m_iFunOrder == 62 && m_iMainMenu_62 % 3 == 1)
            {
                if (m_iMainMenu_62 < 5)
                {
                    m_iMainMenu_62 += 3;
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 63 && m_iMainMenu_63 % 2 == 0)
            {
                if (m_iMainMenu_63 < 9)
                {
                    m_iMainMenu_63 += 2;

                    if (m_iMainMenu_63 > 8)
                    {
                        MainMenu_Change();
                    }
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_63 > 8 && m_iMainMenu_63 < 16)
                {
                    m_iMainMenu_63 += 2;
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 64)
            {
                if (m_iMainMenu_64 < 10)
                {
                    m_iMainMenu_64 += 2;
                    Draw_MainMenu();
                }
            }
        
            Draw_My_Button(0, 7);//绘制按钮按下效果
        }

        private void Button_MouseLeftButtonUp_Stk_down(object sender, MouseButtonEventArgs e)//摇杆下键的左键抬起事件,7
        {
            
            Draw_My_Button(1, 7);//绘制按钮抬起效果
        }
        
        private void left_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//左旋按键，左键按下事件,12
        {
            polygon_ChoseSign_Down.Fill = Brushes.Black;
            m_iRotateDown = -1;
            if (m_iFunOrder == 1)
            {
                Roll_Data(2);
            }


            else if (m_iFunOrder == 2 && m_DispChoose < m_iTotal - 3)
            {
                if (rectangle_ChoseSign_Top.Visibility == Visibility.Visible)
                {
                    rectangle_ChoseSign_Top.Visibility = Visibility.Collapsed;
                    (stackPanel_2_3_Left.Children[0] as TextBlock).Background = Brushes.Black;
                    (stackPanel_2_3_Right.Children[0] as TextBlock).Background = Brushes.Black;
                    (stackPanel_2_3_Mid.Children[0] as TextBlock).Background = Brushes.Black;
                    (stackPanel_2_3_Guard.Children[0] as TextBlock).Background = Brushes.Black;
                    (stackPanel_2_3_Left.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_2_3_Right.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute)));
                    (stackPanel_2_3_Mid.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;
                    (stackPanel_2_3_Guard.Children[0] as TextBlock).Foreground = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/字体颜色.bmp", UriKind.RelativeOrAbsolute))); ;

                }
                else
                {
                    m_DispChoose++;
                    Disp_ShipChoose(m_DispChoose);
                }



            }
            else if (m_iFunOrder == 7)
            {
                m_iKeyBoard_Sel--;
                if (m_iKeyBoard_Sel == -1)
                {
                    m_iKeyBoard_Sel = 65;

                }

                Sel_KeyBoard();

            }
            else if (m_iFunOrder == 20)
            {
                if (m_iGrphic_Sel % 2 == 0 && m_iGrphic_Sel < 15)
                {
                    m_iGrphic_Sel += 2;
                    Draw_Grphic_Setup();
                }
         
                    //RANGE设置
                   else if (m_iGrphic_Sel == 1)
                    {
                        m_fDispRNG /= 2;
                        if (m_fDispRNG < 0.75)
                        {
                            m_fDispRNG = 24f;
                        }
                    }
                    //BEARING设置
                    else if (m_iGrphic_Sel == 3)
                    {
                        if (m_iBRG_Disp == 0)
                        {
                            m_iBRG_Disp = 1;
                        }
                        else
                        {
                            m_iBRG_Disp = 0;
                        }
                    }
                    //SORT设置
                    else if (m_iGrphic_Sel == 5)
                    {
                        m_iSORT_Disp--;
                        if (m_iSORT_Disp < 0)
                        {
                            m_iSORT_Disp = 2;
                        }
                    }
                    //GUARD RNG设置
                    else if (m_iGrphic_Sel == 7)
                    {
                        m_fGuardRNG-=0.1f;
                        if (m_fGuardRNG < 0)
                        {
                            m_fGuardRNG = 99;
                        }
                    }
                    //NUMBER OF SHIPS DISP.设置
                    else if (m_iGrphic_Sel == 9)
                    {
                        if (m_iNumOfShip == 22)
                        {
                            m_iNumOfShip = 128;
                        }
                        else if (m_iNumOfShip == 32)
                        {
                            m_iNumOfShip = 22;
                        }
                        else if (m_iNumOfShip == 64)
                        {
                            m_iNumOfShip = 32;
                        }
                        else if (m_iNumOfShip == 128)
                        {
                            m_iNumOfShip = 64;
                        }
                    }
                    //CONTRAST设置
                    else if (m_iGrphic_Sel == 11)
                    {
                        m_iContrast--;
                        if (m_iContrast < 0)
                        {
                            m_iContrast = 13;
                        }
                    }
                    //AUTO RNG设置
                    else if (m_iGrphic_Sel == 13)
                    {
                        if (m_bAutoRNG == false)
                        {
                            m_bAutoRNG = true;
                            m_fDispRNG = float.Parse(m_dRNG[m_iTotal - 1].ToString("0.0"));
                        }
                        else
                        {
                            m_bAutoRNG = false;
                            m_fDispRNG = 1.5f;
                        }
                    }
                    Draw_Grphic_Setup();
                
            }
            else if (m_iFunOrder == 3 && m_Detail_Choose <4)//数据翻页，船舶详细信息界面
            {
                m_Detail_Choose++;
                Data_Display();

            }
            else if (m_iFunOrder == 4)//绘制高亮，主界面菜单
            {
                if (m_iSelect > -1 && m_iSelect < 5)
                {
                    m_iSelect += 1;
                    Draw_Select();
                }
       
                }
            else if (m_iFunOrder == 420)
            {
                if (m_iOwnpos < 1)
                {
                    m_iOwnpos++;
                    OwnposDisp_Select();
                }
            }
            else if (m_iFunOrder == 41 && m_iList_Select < 2)//List菜单高亮
            {
                m_iList_Select++;
                Draw_Select();
            } 
            else if (m_iFunOrder == 43 && m_Detail_Choose < 1)//数据翻页，本船详细信息界面
            {
                m_Detail_Choose++;
                Data_Display();
            }
            else if (m_iFunOrder == 411 && m_iList_Select < 4)
            {
                m_iList_Select++;
                Draw_Select();
            }
            else if (m_iFunOrder == 412 && m_iList_Select < 7)
            {
                m_iList_Select++;
                Draw_Select();
            }
            else if (m_iFunOrder == 413 && m_iList_Select < 9)
            {
                m_iList_Select++;
                Draw_Select();
            }
            else if (m_iFunOrder == 5)
            {
                m_iKeyBoard_Sel--;
                if (m_iKeyBoard_Sel == -1)
                {
                    m_iKeyBoard_Sel = 65;

                }

                Sel_KeyBoard();

            }
            else if (m_iFunOrder == 6)
            {
                if (m_iMainMenu_Sel < 9)
                {
                    m_iMainMenu_Sel += 2;
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 70)
            {
                m_iKeyBoard_Sel--;
                if (m_iKeyBoard_Sel == -1)
                {
                    m_iKeyBoard_Sel = 65;

                }

                Sel_KeyBoard();

            }
            else if (m_iMainMenu_60 % 2 == 0 && m_iFunOrder == 60)
            {
                if (m_iMainMenu_60 < 9)
                {
                    m_iMainMenu_60 += 2;

                    if (m_iMainMenu_60 > 8)
                    {
                        MainMenu_Change();
                    }
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 > 8 && m_iMainMenu_60 < 15)
                {
                    m_iMainMenu_60 += 2;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 > 15 && m_iMainMenu_60 < 21)
                {
                    m_iMainMenu_60 += 2;
                    Draw_MainMenu();
                }
            }
            if (m_iFunOrder == 60)
            {
                //NAV STATUS设置
                if (m_iMainMenu_60 == 1)
                {
                    m_iOwnNavStatus++;
                    if (m_iOwnNavStatus > 11)
                    {
                        m_iOwnNavStatus = 0;
                    }
                    NavStatus_Setup();
                }
                else if (m_iMainMenu_60 == 7)
                {
                    m_fOwnDraught-=0.1f;
                    if (m_fOwnDraught < 0)
                    {
                        m_fOwnDraught = 26;
                    }
                    m_strOwnshipData[16] = m_fOwnDraught.ToString("0.0");
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = m_strOwnshipData[16] + "M";
                }
                else if (m_iMainMenu_60 == 9)
                {
                    m_iOwnCargoStatus++;
                    if (m_iOwnCargoStatus > 5)
                    {
                        m_iOwnCargoStatus = 0;
                    }
                    CargoStatus_Setup();
                }
                else if (m_iMainMenu_60 == 15)
                {
                    m_iPersons--;
                    if (m_iPersons < 0)
                    {
                        m_iPersons = 50;
                    }
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = m_iPersons.ToString();
                }
                else if (m_iMainMenu_60 == 17)
                {
                    m_iHeightKeel-=0.1f;
                    if (m_iHeightKeel < 0)
                    {
                        m_iHeightKeel = 100;
                    }
                    (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = m_iHeightKeel.ToString("0.0") + "M";
                }
            }
            else if (m_iFunOrder == 64)
            {
                if (m_iMainMenu_64 < 10)
                {
                    m_iMainMenu_64 += 2;
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 61)
            {
                if (m_iMainMenu_61 < 8)
                {
                    m_iMainMenu_61 += 2;
                    Draw_MainMenu();
                }
            
            }
            else if (m_iFunOrder == 613&&m_iIntRogationSel<9)
            {
                if (m_iIntRogationSel == 0)
                {
                    m_iMMSiSel++;
                    if (m_iMMSiSel > m_iTotal)
                    {
                        m_iMMSiSel = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = "1.DESTINATION ID:" + m_strData[m_iMMSiSel, 3];
                }
                else if (m_iIntRogationSel == 2)
                {
                    if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                }
                else if (m_iIntRogationSel == 4)
                {
                    if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "NONE")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "NONE";
                    }
                }
                else if (m_iIntRogationSel == 6)
                {
                
                    m_iMMSiSel++;
                    if (m_iMMSiSel > m_iTotal)
                    {
                        m_iMMSiSel = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "2.DESTINATION ID:" + m_strData[m_iMMSiSel, 3];
                }
                else if (m_iIntRogationSel == 8)
                {
                    if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "NONE")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "NONE";
                    }
                }

            }
            else if (m_iFunOrder == 603)
            {
                if (m_iETAChoose == 0)
                {
                    m_iMonth--;
                    if (m_iMonth < 1)
                    {
                        m_iMonth = 12;
                    }
                    ETA_Edit();
                }
                else if (m_iETAChoose == 1)
                {
                    m_iDay--;
                    ETA_Edit();
                }
                else if (m_iETAChoose == 2)
                {
                    m_iHour--;
                    if (m_iHour < 0)
                    {
                        m_iHour = 23;
                    }
                    (stackPanel_ETA_Edit.Children[2] as TextBlock).Text = m_iHour.ToString("00");
                }
                else if (m_iETAChoose == 3)
                {
                    m_iMinute--;
                    if (m_iMinute < 0)
                    {
                        m_iMinute = 59;
                    }
                    (stackPanel_ETA_Edit.Children[3] as TextBlock).Text = m_iMinute.ToString("00");
                }
            }
            else if (m_iFunOrder == 606)
            {
                if (m_iWpChoose < 5)
                {
                    m_iWpChoose++;
                    Draw_WayPoint();
                }
                else if (m_iWpChoose < 10)
                {
                    m_iWpChoose++;
                    Draw_WayPoint();
                }
            }
            else if (m_iFunOrder == 6060)
            {
                //经纬度属性设置
                if (m_iWpEditChoose == 0)
                {
                    if ((warppanel_WayPointEdit.Children[0] as TextBlock).Text == "N")
                    {
                        (warppanel_WayPointEdit.Children[0] as TextBlock).Text = "S";
                    }
                    else
                    {
                        (warppanel_WayPointEdit.Children[0] as TextBlock).Text = "N";
                    }
                }
                else if (m_iWpEditChoose == 8)
                {
                    if ((warppanel_WayPointEdit.Children[8] as TextBlock).Text == "E")
                    {
                        (warppanel_WayPointEdit.Children[8] as TextBlock).Text = "W";
                    }
                    else
                    {
                        (warppanel_WayPointEdit.Children[8] as TextBlock).Text = "E";
                    }
                }
                else if (m_iWpEditChoose == 1)
                {
                    m_iLAT_Degree_Ten--;
                    if (m_iLAT_Degree_Ten < 0)
                    {
                        m_iLAT_Degree_Ten = 9;
                    }
                    (warppanel_WayPointEdit.Children[1] as TextBlock).Text = m_iLAT_Degree_Ten.ToString();
                }
                else if (m_iWpEditChoose == 2)
                {
                    m_iLAT_Degree_Unit--;
                    if (m_iLAT_Degree_Unit < 0)
                    {
                        m_iLAT_Degree_Unit = 9;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Degree_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[2] as TextBlock).Text = m_iLAT_Degree_Unit.ToString();
                }
                else if (m_iWpEditChoose == 3)
                {
                    m_iLAT_Minute_Ten--;
                    if (m_iLAT_Minute_Ten < 0)
                    {
                        m_iLAT_Minute_Ten = 6;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Ten = 0;
                    }
                    (warppanel_WayPointEdit.Children[3] as TextBlock).Text = m_iLAT_Minute_Ten.ToString();
                }
                else if (m_iWpEditChoose == 4)
                {
                    m_iLAT_Minute_Unit--;
                    if (m_iLAT_Minute_Unit < 0)
                    {
                        m_iLAT_Minute_Unit = 9;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[4] as TextBlock).Text = m_iLAT_Minute_Unit.ToString();
                }
                else if (m_iWpEditChoose == 5)
                {
                    m_iLAT_Minute_Tenths--;
                    if (m_iLAT_Minute_Tenths < 0)
                    {
                        m_iLAT_Minute_Tenths = 9;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Tenths = 0;
                    }
                    (warppanel_WayPointEdit.Children[5] as TextBlock).Text = m_iLAT_Minute_Tenths.ToString();
                }
                else if (m_iWpEditChoose == 6)
                {
                    m_iLAT_Minute_Percent--;
                    if (m_iLAT_Minute_Percent < 0)
                    {
                        m_iLAT_Minute_Percent = 9;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Percent = 0;
                    }
                    (warppanel_WayPointEdit.Children[6] as TextBlock).Text = m_iLAT_Minute_Percent.ToString();
                }
                else if (m_iWpEditChoose == 7)
                {
                    m_iLAT_Minute_Thousands--;
                    if (m_iLAT_Minute_Thousands < 0)
                    {
                        m_iLAT_Minute_Thousands = 9;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Thousands = 0;
                    }
                    (warppanel_WayPointEdit.Children[7] as TextBlock).Text = m_iLAT_Minute_Thousands.ToString();
                }
                else if (m_iWpEditChoose == 9)
                {
                    m_iLON_Degree_Hundred--;
                    if (m_iLON_Degree_Hundred < 0)
                    {
                        m_iLON_Degree_Hundred = 1;
                    }
                    (warppanel_WayPointEdit.Children[9] as TextBlock).Text = m_iLON_Degree_Hundred.ToString();
                }
                else if (m_iWpEditChoose == 10)
                {
                    m_iLON_Degree_Ten--;
                    if (m_iLON_Degree_Ten < 0)
                    {
                        m_iLON_Degree_Ten = 9;
                        if (m_iLON_Degree_Hundred == 1)
                        {
                            m_iLON_Degree_Ten = 8;
                        }
                    }


                    (warppanel_WayPointEdit.Children[10] as TextBlock).Text = m_iLON_Degree_Ten.ToString();
                }
                else if (m_iWpEditChoose == 11)
                {
                    m_iLON_Degree_Unit--;
                    if (m_iLON_Degree_Unit < 0)
                    {
                        m_iLON_Degree_Unit = 9;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Degree_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[11] as TextBlock).Text = m_iLON_Degree_Unit.ToString();
                }
                else if (m_iWpEditChoose == 12)
                {
                    m_iLON_Minute_Ten--;
                    if (m_iLON_Minute_Ten < 0)
                    {
                        m_iLON_Minute_Ten = 6;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Ten = 0;
                    }
                    (warppanel_WayPointEdit.Children[12] as TextBlock).Text = m_iLON_Minute_Ten.ToString();
                }
                else if (m_iWpEditChoose == 13)
                {
                    m_iLON_Minute_Unit--;
                    if (m_iLON_Minute_Unit < 0)
                    {
                        m_iLON_Minute_Unit = 9;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[13] as TextBlock).Text = m_iLON_Minute_Unit.ToString();
                }
                else if (m_iWpEditChoose == 14)
                {
                    m_iLON_Minute_Tenths--;
                    if (m_iLON_Minute_Tenths < 0)
                    {
                        m_iLON_Minute_Tenths = 9;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Tenths = 0;
                    }
                    (warppanel_WayPointEdit.Children[14] as TextBlock).Text = m_iLON_Minute_Tenths.ToString();
                }
                else if (m_iWpEditChoose == 15)
                {
                    m_iLON_Minute_Percent--;
                    if (m_iLON_Minute_Percent < 0)
                    {
                        m_iLON_Minute_Percent = 9;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Percent = 0;
                    }
                    (warppanel_WayPointEdit.Children[15] as TextBlock).Text = m_iLON_Minute_Percent.ToString();
                }
                else if (m_iWpEditChoose == 16)
                {
                    m_iLON_Minute_Thousands--;
                    if (m_iLON_Minute_Thousands < 0)
                    {
                        m_iLON_Minute_Thousands = 9;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Thousands = 0;
                    }
                    (warppanel_WayPointEdit.Children[16] as TextBlock).Text = m_iLON_Minute_Thousands.ToString();
                }
            }
            else if (m_iFunOrder == 610)
            {
                if (m_iTXSel < 3)
                {
                    m_iTXSel++;
                    Draw_TX();
                }
                else if (m_iTXSel < 8 && m_iTXSel != 4)
                {
                    m_iTXSel += 2;
                    Draw_TX();
                }
                else
                {
                    m_iFunOrder = 70;
                    Draw_My_Screen(m_Struct_No1);
                    Data_Display();
                    m_iKeyBoard_Sel = 0;
                    Sel_KeyBoard();
                }

            }
            else if (m_iFunOrder == 611 || m_iFunOrder == 612)
            {
                if (m_iTXSend_Sel < 9)
                {
                    m_iTXSend_Sel++;
                }
                TX_Send_Receive_Disp();
            }
            else if (m_iFunOrder == 613&&m_iIntRogationSel>9)
            {
                if (m_iIntRogationSel < 19)
                {
                    m_iIntRogationSel += 2;
                    Draw_IntRogation();
                }
            }

            else if (m_iFunOrder == 62 && m_iMainMenu_62 % 3 == 1)
            {
                if (m_iMainMenu_62 < 5)
                {
                    m_iMainMenu_62 += 3;
                    Draw_MainMenu();
                }

            }
            else if (m_iFunOrder == 62)
            {
                if (m_iMainMenu_62 == 2)
                {
                    m_fGuardRNG -= 0.1f;
                    if (m_fGuardRNG < 0)
                    {
                        m_fGuardRNG = 99;
                    }
                }
                else if (m_iMainMenu_62 == 5)
                {
                    if (m_fLostRNG > m_fGuardRNG + 1)
                    {
                        m_fLostRNG -= 0.1f;
                    }

                    if (m_fLostRNG < 0)
                    {
                        m_fLostRNG = 99;
                    }
                }
                Alarm_Setup();
            }

            else if (m_iFunOrder == 63 && m_iMainMenu_63 % 2 == 0)
            {
                if (m_iMainMenu_63 < 9)
                {
                    m_iMainMenu_63 += 2;

                    if (m_iMainMenu_63 > 8)
                    {
                        MainMenu_Change();
                    }
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_63 > 8 && m_iMainMenu_63 < 16)
                {
                    m_iMainMenu_63 += 2;
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 63 && m_iMainMenu_63 % 2 != 0)
            {
                if (m_iMainMenu_63 == 1)
                {
                    m_iContrast--;
                    if (m_iContrast < 0)
                    {
                        m_iContrast = 13;
                    }
                    (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = m_iContrast.ToString();
                }
                else if (m_iMainMenu_63 == 3)
                {
                    if (m_bUTC == false)
                    {
                        m_bUTC = true;
                        (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "OFF";
                    }
                    else
                    {
                        m_bUTC = false;
                        (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "ON";
                    }
                }
                else if (m_iMainMenu_63 == 7)
                {
                    if ((stackPanel_6_Mainmenu.Children[7] as TextBlock).Text == "AUTO")
                    {
                        (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "MANUAL";
                    }
                    else
                    {
                        (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "AUTO";
                    }
                }
                else if (m_iMainMenu_63 == 8)
                {
                    if (m_bSound == false)
                    {
                        m_bSound = true;
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.BUZZER         ：ON";
                    }
                    else
                    {
                        m_bSound = false;
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.BUZZER         ：OFF";
                    }
                }
            
           
          
            }
            m_iQuickEdit = -1;
            m_QuickEdit.Start();
            //绘制旋钮

            Count_Rotate--;
            if (Count_Rotate == -1)
            {
                Count_Rotate = 19;
            }

            RG_123.Rect = new Rect(Count_Rotate * 157, 0, 157, 157);
            image11.Margin = new Thickness(43 - Count_Rotate * 157, 5, 0, 0);
        }
        
        private void left_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)//左旋按键，左键抬起事件,12
        {
            m_QuickEdit.Stop();
            m_iRotateDown = 0;
            left.Background = new ImageBrush();
           

        }
      
        private void right_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//右旋按键，左键按下事件,11
        {
            polygon_ChoseSign_Up.Fill = Brushes.Black;
            m_iRotateDown = 1;
            if (m_iFunOrder == 1)
            {
                if (m_Count_Choose == 0 && rectangle_ChoseSign_Top.Visibility == Visibility.Collapsed && m_Count_Choose_Up == 0)
                {
                    Draw_Choose(0);
                }
                else if (rectangle_ChoseSign_Top.Visibility == Visibility.Visible)
                {
                    TitleDisplay(m_iTitle_Choose);
                    m_iFunOrder = 4;
                    m_iSelect = 0;
                    Draw_My_Screen(m_Struct_No1);
                    Data_Display();
                    Draw_Select();
                }
                Roll_Data(1);


            }

            else if (m_iFunOrder == 2)//绘制高亮，DSPL界面
            {
                if (m_DispChoose == 0)
                {
                    rectangle_ChoseSign_Top.Visibility = Visibility.Visible;
                    (stackPanel_2_3_Left.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Right.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Mid.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Guard.Children[0] as TextBlock).Background = Brushes.Transparent;
                    (stackPanel_2_3_Left.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_2_3_Right.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_2_3_Mid.Children[0] as TextBlock).Foreground = Brushes.Black;
                    (stackPanel_2_3_Guard.Children[0] as TextBlock).Foreground = Brushes.Black;

                }
                if (m_DispChoose > 0)
                {
                    m_DispChoose--;
                    Disp_ShipChoose(m_DispChoose);
                }



            }
            else if (m_iFunOrder == 20)
            {
                if (m_iGrphic_Sel % 2 == 0 && m_iGrphic_Sel > 0)
                {
                    m_iGrphic_Sel -= 2;
                    Draw_Grphic_Setup();
                }
         
                    //RANGE设置
                else if (m_iGrphic_Sel == 1)
                    {
                        m_fDispRNG *= 2;
                        if (m_fDispRNG > 24)
                        {
                            m_fDispRNG = 0.75f;
                        }
                    }
                    //BEARING设置
                    else if (m_iGrphic_Sel == 3)
                    {
                        if (m_iBRG_Disp == 0)
                        {
                            m_iBRG_Disp = 1;
                        }
                        else
                        {
                            m_iBRG_Disp = 0;
                        }
                    }
                    //SORT设置
                    else if (m_iGrphic_Sel == 5)
                    {
                        m_iSORT_Disp++;
                        if (m_iSORT_Disp > 2)
                        {
                            m_iSORT_Disp = 0;
                        }
                    }
                    //GUARD RNG设置
                    else if (m_iGrphic_Sel == 7)
                    {
                        m_fGuardRNG+=0.1f;
                        if (m_fGuardRNG > 99)
                        {
                            m_fGuardRNG = 0;
                        }
                    }
                    //NUMBER OF SHIPS DISP.设置
                    else if (m_iGrphic_Sel == 9)
                    {
                        if (m_iNumOfShip == 128)
                        {
                            m_iNumOfShip = 22;
                        }
                        else if (m_iNumOfShip == 22)
                        {
                            m_iNumOfShip = 32;
                        }
                        else if (m_iNumOfShip == 32)
                        {
                            m_iNumOfShip = 64;
                        }
                        else if (m_iNumOfShip == 64)
                        {
                            m_iNumOfShip = 128;
                        }
                    }
                    //CONTRAST设置
                    else if (m_iGrphic_Sel == 11)
                    {
                        m_iContrast++;
                        if (m_iContrast > 13)
                        {
                            m_iContrast = 0;
                        }
                    }
                    //AUTO RNG设置
                    else if (m_iGrphic_Sel == 13)
                    {
                        if (m_bAutoRNG == false)
                        {
                            m_bAutoRNG = true;
                            m_fDispRNG = float.Parse(m_dRNG[m_iTotal - 1].ToString("0.0"));
                        }
                        else
                        {
                            m_bAutoRNG = false;
                            m_fDispRNG = 1.5f;
                        }
                    }
                    Draw_Grphic_Setup();
                
            }
            else if (m_iFunOrder == 3 && m_Detail_Choose > 0)//数据翻页，船舶详细信息界面
            {
                m_Detail_Choose--;
                Data_Display();


            }
            else if (m_iFunOrder == 1 && polygon_ChoseSign_Up.Visibility == Visibility.Collapsed)
            {
                Draw_Choose(0);
            }
            else if (m_iFunOrder == 4)//绘制高亮，主界面菜单
            {
                if (m_iSelect > 0 && m_iSelect < 6)
                {
                    m_iSelect -= 1;
                    Draw_Select();
                }
            }
            else if (m_iFunOrder == 420)
            {
                if (m_iOwnpos > 0)
                {
                    m_iOwnpos--;
                    OwnposDisp_Select();
                }
            }
            else if (m_iFunOrder == 41 && m_iList_Select > 0)//List菜单高亮
            {
                m_iList_Select--;
                Draw_Select();
            } 

             else if (m_iFunOrder == 43 && m_Detail_Choose > 0)//数据翻页，本船详细信息界面
              {
                    m_Detail_Choose--;
                    Data_Display();
                }
            else if (m_iFunOrder == 411 && m_iList_Select > 3)
            {
                m_iList_Select--;
                Draw_Select();
            }
            else if (m_iFunOrder == 412 && m_iList_Select > 5)
            {
                m_iList_Select--;
                Draw_Select();
            }
            else if (m_iFunOrder == 413 && m_iList_Select > 8)
            {
                m_iList_Select--;
                Draw_Select();
            }
            else if (m_iFunOrder == 5)
            {
                m_iKeyBoard_Sel++;
                if (m_iKeyBoard_Sel == 66)
                {
                    m_iKeyBoard_Sel = 0;

                }

                Sel_KeyBoard();

            }
            else if (m_iFunOrder == 6)
            {
                if (m_iMainMenu_Sel > 1)
                {
                    m_iMainMenu_Sel -= 2;
                    Draw_MainMenu();
                }
            }

            else if (m_iMainMenu_60 % 2 == 0 && m_iFunOrder == 60)
            {
                if (m_iMainMenu_60 > 1 && m_iMainMenu_60 < 9)
                {
                    m_iMainMenu_60 -= 2;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 > 8)
                {
                    m_iMainMenu_60 -= 2;
                    if (m_iMainMenu_60 < 9)
                    {
                        MainMenu_Change();
                    }
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 61)
            {
                if (m_iMainMenu_61 > 1)
                {
                    m_iMainMenu_61 -= 2;
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 603)
            {
                if (m_iETAChoose == 0)
                {
                    m_iMonth++;
                    if (m_iMonth > 12)
                    {
                        m_iMonth = 1;
                    }
                    ETA_Edit();
                }
                else if (m_iETAChoose == 1)
                {
                    m_iDay++;
                    ETA_Edit();
                }
                else if (m_iETAChoose == 2)
                {
                    m_iHour++;
                    if (m_iHour > 23)
                    {
                        m_iHour = 0;
                    }
                    (stackPanel_ETA_Edit.Children[2] as TextBlock).Text = m_iHour.ToString("00");
                }
                else if (m_iETAChoose == 3)
                {
                    m_iMinute++;
                    if (m_iMinute > 59)
                    {
                        m_iMinute = 0;
                    }
                    (stackPanel_ETA_Edit.Children[3] as TextBlock).Text = m_iMinute.ToString("00");
                }
            }
            else if (m_iFunOrder == 606)
            {
                if (m_iWpChoose > 1)
                {
                    m_iWpChoose--;
                    Draw_WayPoint();
                }
            }
            else if (m_iFunOrder == 6060)
            {
                //经纬度属性设置
                if (m_iWpEditChoose == 0)
                {
                    if ((warppanel_WayPointEdit.Children[0] as TextBlock).Text == "N")
                    {
                        (warppanel_WayPointEdit.Children[0] as TextBlock).Text = "S";
                    }
                    else
                    {
                        (warppanel_WayPointEdit.Children[0] as TextBlock).Text = "N";
                    }
                }
                else if (m_iWpEditChoose == 8)
                {
                    if ((warppanel_WayPointEdit.Children[8] as TextBlock).Text == "E")
                    {
                        (warppanel_WayPointEdit.Children[8] as TextBlock).Text = "W";
                    }
                    else
                    {
                        (warppanel_WayPointEdit.Children[8] as TextBlock).Text = "E";
                    }
                }
                else if (m_iWpEditChoose == 1)
                {
                    m_iLAT_Degree_Ten++;
                    if (m_iLAT_Degree_Ten > 9)
                    {
                        m_iLAT_Degree_Ten = 0;
                    }
                    (warppanel_WayPointEdit.Children[1] as TextBlock).Text = m_iLAT_Degree_Ten.ToString();
                }
                else if (m_iWpEditChoose == 2)
                {
                    m_iLAT_Degree_Unit++;
                    if (m_iLAT_Degree_Unit > 9)
                    {
                        m_iLAT_Degree_Unit = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Degree_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[2] as TextBlock).Text = m_iLAT_Degree_Unit.ToString();
                }
                else if (m_iWpEditChoose == 3)
                {
                    m_iLAT_Minute_Ten++;
                    if (m_iLAT_Minute_Ten > 6)
                    {
                        m_iLAT_Minute_Ten = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Ten = 0;
                    }
                    (warppanel_WayPointEdit.Children[3] as TextBlock).Text = m_iLAT_Minute_Ten.ToString();
                }
                else if (m_iWpEditChoose == 4)
                {
                    m_iLAT_Minute_Unit++;
                    if (m_iLAT_Minute_Unit > 9)
                    {
                        m_iLAT_Minute_Unit = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[4] as TextBlock).Text = m_iLAT_Minute_Unit.ToString();
                }
                else if (m_iWpEditChoose == 5)
                {
                    m_iLAT_Minute_Tenths++;
                    if (m_iLAT_Minute_Tenths > 9)
                    {
                        m_iLAT_Minute_Tenths = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Tenths = 0;
                    }
                    (warppanel_WayPointEdit.Children[5] as TextBlock).Text = m_iLAT_Minute_Tenths.ToString();
                }
                else if (m_iWpEditChoose == 6)
                {
                    m_iLAT_Minute_Percent++;
                    if (m_iLAT_Minute_Percent > 9)
                    {
                        m_iLAT_Minute_Percent = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Percent = 0;
                    }
                    (warppanel_WayPointEdit.Children[6] as TextBlock).Text = m_iLAT_Minute_Percent.ToString();
                }
                else if (m_iWpEditChoose == 7)
                {
                    m_iLAT_Minute_Thousands++;
                    if (m_iLAT_Minute_Thousands > 9)
                    {
                        m_iLAT_Minute_Thousands = 0;
                    }
                    if (m_iLAT_Degree_Ten == 9)
                    {
                        m_iLAT_Minute_Thousands = 0;
                    }
                    (warppanel_WayPointEdit.Children[7] as TextBlock).Text = m_iLAT_Minute_Thousands.ToString();
                }
                else if (m_iWpEditChoose == 9)
                {
                    m_iLON_Degree_Hundred++;
                    if (m_iLON_Degree_Hundred > 1)
                    {
                        m_iLON_Degree_Hundred = 0;
                    }
                    (warppanel_WayPointEdit.Children[9] as TextBlock).Text = m_iLON_Degree_Hundred.ToString();
                }
                else if (m_iWpEditChoose == 10)
                {
                    m_iLON_Degree_Ten++;
                    if (m_iLON_Degree_Hundred == 1)
                    {
                        if (m_iLON_Degree_Ten > 8)
                        {
                            m_iLON_Degree_Ten = 0;
                        }
                    }
                    else if (m_iLON_Degree_Ten > 9)
                    {
                        m_iLON_Degree_Ten = 0;
                    }

                    (warppanel_WayPointEdit.Children[10] as TextBlock).Text = m_iLON_Degree_Ten.ToString();
                }
                else if (m_iWpEditChoose == 11)
                {
                    m_iLON_Degree_Unit++;
                    if (m_iLON_Degree_Unit > 9)
                    {
                        m_iLON_Degree_Unit = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Degree_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[11] as TextBlock).Text = m_iLON_Degree_Unit.ToString();
                }
                else if (m_iWpEditChoose == 12)
                {
                    m_iLON_Minute_Ten++;
                    if (m_iLON_Minute_Ten > 6)
                    {
                        m_iLON_Minute_Ten = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Ten = 0;
                    }
                    (warppanel_WayPointEdit.Children[12] as TextBlock).Text = m_iLON_Minute_Ten.ToString();
                }
                else if (m_iWpEditChoose == 13)
                {
                    m_iLON_Minute_Unit++;
                    if (m_iLON_Minute_Unit > 9)
                    {
                        m_iLON_Minute_Unit = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Unit = 0;
                    }
                    (warppanel_WayPointEdit.Children[13] as TextBlock).Text = m_iLON_Minute_Unit.ToString();
                }
                else if (m_iWpEditChoose == 14)
                {
                    m_iLON_Minute_Tenths++;
                    if (m_iLON_Minute_Tenths > 9)
                    {
                        m_iLON_Minute_Tenths = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Tenths = 0;
                    }
                    (warppanel_WayPointEdit.Children[14] as TextBlock).Text = m_iLON_Minute_Tenths.ToString();
                }
                else if (m_iWpEditChoose == 15)
                {
                    m_iLON_Minute_Percent++;
                    if (m_iLON_Minute_Percent > 9)
                    {
                        m_iLON_Minute_Percent = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Percent = 0;
                    }
                    (warppanel_WayPointEdit.Children[15] as TextBlock).Text = m_iLON_Minute_Percent.ToString();
                }
                else if (m_iWpEditChoose == 16)
                {
                    m_iLON_Minute_Thousands++;
                    if (m_iLON_Minute_Thousands > 9)
                    {
                        m_iLON_Minute_Thousands = 0;
                    }
                    if (m_iLON_Degree_Hundred == 1 && m_iLON_Degree_Ten == 8)
                    {
                        m_iLON_Minute_Thousands = 0;
                    }
                    (warppanel_WayPointEdit.Children[16] as TextBlock).Text = m_iLON_Minute_Thousands.ToString();
                }
            }
        
            else if (m_iFunOrder == 610)
            {
                if (m_iTXSel > 0 && m_iTXSel < 4)
                {
                    m_iTXSel--;
                    Draw_TX();
                }
                else if (m_iTXSel > 4 && m_iTXSel < 10)
                {
                    m_iTXSel -= 2;
                    Draw_TX();
                }

            }
            else if (m_iFunOrder == 611 || m_iFunOrder == 612)
            {
                if (m_iTXSend_Sel > 0)
                {
                    m_iTXSend_Sel--;
                }
                TX_Send_Receive_Disp();
            }
            else if (m_iFunOrder == 613&&m_iIntRogationSel<9)
            {
                if (m_iIntRogationSel == 0)
                {
                    m_iMMSiSel--;
                    if (m_iMMSiSel < 0)
                    {
                        m_iMMSiSel = m_iTotal;
                    }
                    (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = "1.DESTINATION ID:" + m_strData[m_iMMSiSel, 3];
                }
                else if (m_iIntRogationSel == 2)
                {
                    if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                }
                else if (m_iIntRogationSel == 4)
                {
                    if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "NONE";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "NONE")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                }
                else if (m_iIntRogationSel == 6)
                {

                    m_iMMSiSel++;
                    if (m_iMMSiSel > m_iTotal)
                    {
                        m_iMMSiSel = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "2.DESTINATION ID:" + m_strData[m_iMMSiSel, 3];
                }
                else if (m_iIntRogationSel == 8)
                {
                    if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "NONE";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "NONE")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                }

            }
            else if (m_iFunOrder == 613&&m_iIntRogationSel>9)
            {
                if (m_iIntRogationSel > 0)
                {
                    m_iIntRogationSel -= 2;
                    Draw_IntRogation();
                }
            }
            else if (m_iFunOrder == 62 && m_iMainMenu_62 % 3 == 1)
            {
                if (m_iMainMenu_62 > 1)
                {
                    m_iMainMenu_62 -= 3;
                    Draw_MainMenu();
                }

            }
            if (m_iFunOrder == 60)
            {
                //NAV STATUS设置
                if (m_iMainMenu_60 == 1)
                {
                    m_iOwnNavStatus++;
                    if (m_iOwnNavStatus > 11)
                    {
                        m_iOwnNavStatus = 0;
                    }
                    NavStatus_Setup();
                }
                else if (m_iMainMenu_60 == 7)
                {
                    m_fOwnDraught+=0.1f;
                    if (m_fOwnDraught > 26)
                    {
                        m_fOwnDraught = 0;
                    }
                    m_strOwnshipData[16] = m_fOwnDraught.ToString("0.0");
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = m_strOwnshipData[16] + "M";
                }
                else if (m_iMainMenu_60 == 9)
                {
                    m_iOwnCargoStatus++;
                    if (m_iOwnCargoStatus > 5)
                    {
                        m_iOwnCargoStatus = 0;
                    }
                    CargoStatus_Setup();
                }
                else if (m_iMainMenu_60 == 15)
                {
                    m_iPersons++;
                    if (m_iPersons > 50)
                    {
                        m_iPersons = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = m_iPersons.ToString();
                }
                else if (m_iMainMenu_60 == 17)
                {
                    m_iHeightKeel+=0.1f;
                    if (m_iHeightKeel > 100)
                    {
                        m_iHeightKeel = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = m_iHeightKeel.ToString("0.0") + "M";
                }

            }
            else if (m_iFunOrder == 62)
            {

                if (m_iMainMenu_62 == 2)
                {
                    m_fGuardRNG+=0.1f;
                    if (m_fGuardRNG > 99)
                    {
                        m_fGuardRNG = 0;
                    }
                }
                else if (m_iMainMenu_62 == 5)
                {

                    m_fLostRNG+=0.1f;

                    if (m_fLostRNG > 99)
                    {
                        m_fLostRNG = m_fGuardRNG + 0.1f;
                    }
                }
                Alarm_Setup();
            }

            else if (m_iFunOrder == 63 && m_iMainMenu_63 % 2 == 0)
            {
                if (m_iMainMenu_63 > 1 && m_iMainMenu_63 < 9)
                {
                    m_iMainMenu_63 -= 2;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_63 > 8)
                {
                    m_iMainMenu_63 -= 2;
                    if (m_iMainMenu_63 < 9)
                    {
                        MainMenu_Change();
                    }
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 64)
            {
                if (m_iMainMenu_64 > 1)
                {
                    m_iMainMenu_64 -= 2;
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 7)
            {
                m_iKeyBoard_Sel++;


                Sel_KeyBoard();


            }
            else if (m_iFunOrder == 70)
            {
                m_iKeyBoard_Sel++;


                Sel_KeyBoard();


            }
            else if (m_iFunOrder == 63&&m_iMainMenu_63%2!=0)
            {
                if (m_iMainMenu_63 == 1)
                {
                    m_iContrast++;
                    if (m_iContrast > 13)
                    {
                        m_iContrast = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = m_iContrast.ToString();
                }
                else if (m_iMainMenu_63 == 3)
                {
                    if (m_bUTC == false)
                    {
                        m_bUTC = true;
                        (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "OFF";
                    }
                    else
                    {
                        m_bUTC = false;
                        (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "ON";
                    }
                }
                else if (m_iMainMenu_63 == 7)
                {
                    if ((stackPanel_6_Mainmenu.Children[7] as TextBlock).Text == "AUTO")
                    {
                        (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "MANUAL";
                    }
                    else
                    {
                        (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "AUTO";
                    }
                }
                else if (m_iMainMenu_63 == 8)
                {
                    if (m_bSound == false)
                    {
                        m_bSound = true;
                           (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.BUZZER         ：ON";
                    }
                    else
                    {
                        m_bSound = false;
                            (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.BUZZER         ：OFF";
                    }
                }
            }
            m_iQuickEdit = 1;
            m_QuickEdit.Start();

                //绘制旋钮
              
                Count_Rotate++;
                if (Count_Rotate == 20)
                {
                    Count_Rotate = 0;
                }

                RG_123.Rect = new Rect(Count_Rotate * 157, 0, 157, 157);
                image11.Margin = new Thickness(43 - Count_Rotate * 157, 5, 0, 0);


            }
      
        private void right_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)//右旋按键，左键抬起事件,11
        {
            m_QuickEdit.Stop();
            m_iRotateDown = 0;
            right.Background = new ImageBrush();

        }

        private void Button_MouseLeftButtonDown_Stk_left(object sender, MouseButtonEventArgs e)//摇杆左键的左键按下事件,8
        {
          

            if (m_iFunOrder == 20)
            {
                //RANGE设置
                if (m_iGrphic_Sel == 1)
                {
                    m_fDispRNG /= 2;
                    if (m_fDispRNG < 0.75)
                    {
                        m_fDispRNG = 24f;
                    }
                }
                //BEARING设置
                else if (m_iGrphic_Sel == 3)
                {
                    if (m_iBRG_Disp == 0)
                    {
                        m_iBRG_Disp = 1;
                    }
                    else
                    {
                        m_iBRG_Disp = 0;
                    }
                }
                //SORT设置
                else if (m_iGrphic_Sel == 5)
                {
                    m_iSORT_Disp--;
                    if (m_iSORT_Disp < 0)
                    {
                        m_iSORT_Disp = 2;
                    }
                }
                //GUARD RNG设置
                else if (m_iGrphic_Sel == 7)
                {
                    m_fGuardRNG-=0.1f;
                    if (m_fGuardRNG < 0)
                    {
                        m_fGuardRNG = 99;
                    }
                }
                //NUMBER OF SHIPS DISP.设置
                else if (m_iGrphic_Sel == 9)
                {                
                    if (m_iNumOfShip== 22)
                    {
                        m_iNumOfShip = 128;
                    }
                    else if (m_iNumOfShip == 32)
                    {
                        m_iNumOfShip = 22;
                    }
                    else if (m_iNumOfShip == 64)
                    {
                        m_iNumOfShip = 32;
                    }
                    else if (m_iNumOfShip == 128)
                    {
                        m_iNumOfShip = 64;
                    }
                }
                //CONTRAST设置
                else if (m_iGrphic_Sel == 11)
                {
                    m_iContrast--;
                    if (m_iContrast < 0)
                    {
                        m_iContrast = 13;
                    }
                }
                //AUTO RNG设置
                else if (m_iGrphic_Sel == 13)
                {
                    if (m_bAutoRNG == false)
                    {
                        m_bAutoRNG = true;
                        m_fDispRNG = float.Parse(m_dRNG[m_iTotal - 1].ToString("0.0"));
                    }
                    else
                    {
                        m_bAutoRNG = false;
                        m_fDispRNG = 1.5f;
                    }
                }
                Draw_Grphic_Setup();
            }
            if (m_iFunOrder == 60)
            {
                //NAV STATUS设置
                if (m_iMainMenu_60 == 1)
                {
                    m_iOwnNavStatus--;
                    if (m_iOwnNavStatus < 0)
                    {
                        m_iOwnNavStatus = 11;
                    }
                    NavStatus_Setup();
                }
                else if (m_iMainMenu_60 == 7)
                {
                    m_fOwnDraught-=0.1f;
                    if (m_fOwnDraught < 0)
                    {
                        m_fOwnDraught = 25;
                    }
                    m_strOwnshipData[16] = m_fOwnDraught.ToString("0.0");
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = m_strOwnshipData[16] + "M";
                }
                else if (m_iMainMenu_60 == 9)
                {
                    m_iOwnCargoStatus--;
                    if (m_iOwnCargoStatus < 0)
                    {
                        m_iOwnCargoStatus = 5;
                    }
                    CargoStatus_Setup();
                }
                else if (m_iMainMenu_60 == 15)
                {
                    m_iPersons--;
                    if (m_iPersons < 0)
                    {
                        m_iPersons = 50;
                    }
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = m_iPersons.ToString();
                }
                else if (m_iMainMenu_60 == 17)
                {
                    m_iHeightKeel-=0.1f;
                    if (m_iHeightKeel < 0)
                    {
                        m_iHeightKeel = 100;
                    }
                    (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = m_iHeightKeel.ToString("0.0") + "M";
                }


            }
            else if (m_iFunOrder == 6060)
            {
                if (m_iWpEditChoose > 0)
                {
                    m_iWpEditChoose--;
                    Draw_WayPointEdit();
                }
            }
            else if (m_iFunOrder == 603)
            {
                if (m_iETAChoose == 0)
                {
                    m_iMonth--;
                    if (m_iMonth < 1)
                    {
                        m_iMonth = 12;
                    }
                    ETA_Edit();
                }
                else if (m_iETAChoose == 1)
                {
                    m_iDay--;
                    ETA_Edit();
                }
                else if (m_iETAChoose == 2)
                {
                    m_iHour--;
                    if (m_iHour < 0)
                    {
                        m_iHour = 23;
                    }
                    (stackPanel_ETA_Edit.Children[2] as TextBlock).Text = m_iHour.ToString("00");
                }
                else if (m_iETAChoose == 3)
                {
                    m_iMinute--;
                    if (m_iMinute < 0)
                    {
                        m_iMinute = 59;
                    }
                    (stackPanel_ETA_Edit.Children[3] as TextBlock).Text = m_iMinute.ToString("00");
                }
            }
            else if (m_iFunOrder == 606)
            {
                if ((stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Text == "CLR")
                {
                    (stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Text = " " + m_iWpChoose.ToString() + ".";
                }
                else
                {
                    (stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Text = "CLR";
                }
            }
            else if (m_iFunOrder == 610)
            {
                if (m_iTXSel == 0)
                {
                    if ((stackPanel_TX.Children[0] as TextBlock).Text == "1.FORMAT : " + "ADDRESSED")
                    {
                        (stackPanel_TX.Children[0] as TextBlock).Text = "1.FORMAT : " + "BROADCAST";
                    }
                    else
                    {
                        (stackPanel_TX.Children[0] as TextBlock).Text = "1.FORMAT : " + "ADDRESSED";
                    }
                }
                else if (m_iTXSel == 1)
                {
                    m_iMMSiSel--;
                    if (m_iMMSiSel <0)
                    {
                        m_iMMSiSel = m_iTotal;
                    }
                    (stackPanel_TX.Children[1] as TextBlock).Text = "    MMSI : " + m_strData[m_iMMSiSel, 3];
                }
                else if (m_iTXSel == 2)
                {
                    if ((stackPanel_TX.Children[2] as TextBlock).Text == "2.CATAGORY : " + "ROUTINE")
                    {
                        (stackPanel_TX.Children[2] as TextBlock).Text = "2.CATAGORY : " + "SAFETY";
                    }
                    else
                    {
                        (stackPanel_TX.Children[2] as TextBlock).Text = "2.CATAGORY : " + "ROUTINE";
                    }
                }
                else if (m_iTXSel == 3)
                {
                    if ((stackPanel_TX.Children[4] as TextBlock).Text == "TEXT")
                    {
                        (stackPanel_TX.Children[4] as TextBlock).Text = "CAPABILITY INTERROGATE";
                    }
                    else
                    {
                        (stackPanel_TX.Children[4] as TextBlock).Text = "TEXT";
                    }
                }
                else if (m_iTXSel == 5)
                {
                    if ((stackPanel_TX.Children[5] as TextBlock).Text == "4.REPLY : " + "ON")
                    {
                        (stackPanel_TX.Children[5] as TextBlock).Text = "4.REPLY : " + "OFF";
                    }
                    else
                    {
                        (stackPanel_TX.Children[5] as TextBlock).Text = "4.REPLY : " + "ON";
                    }
                }
                else if (m_iTXSel == 7)
                {
                    if ((stackPanel_TX.Children[7] as TextBlock).Text == "5.CH : " + "A/B")
                    {
                        (stackPanel_TX.Children[7] as TextBlock).Text = "5.CH : " + "AUTO";

                    }
                    else if ((stackPanel_TX.Children[7] as TextBlock).Text == "5.CH : " + "AUTO")
                    {
                        (stackPanel_TX.Children[7] as TextBlock).Text = "5.CH : " + "B";
                    }
                    else if ((stackPanel_TX.Children[7] as TextBlock).Text == "5.CH : " + "B")
                    {
                        (stackPanel_TX.Children[7] as TextBlock).Text = "5.CH : " + "A";
                    }
                    else
                    {
                        (stackPanel_TX.Children[7] as TextBlock).Text = "5.CH : " + "A/B";
                    }
                }
                else if (m_iTXSel == 9)
                {
                    if ((stackPanel_TX.Children[9] as TextBlock).Text == "6.NUMBER OF RETRY : " + "3")
                    {
                        (stackPanel_TX.Children[9] as TextBlock).Text = "6.NUMBER OF RETRY : " + "0";
                    }
                    else
                    {
                        (stackPanel_TX.Children[9] as TextBlock).Text = "6.NUMBER OF RETRY : " + "3";
                    }
                }
            }
            else if (m_iFunOrder == 613)
            {
                if (m_iIntRogationSel == 0)
                {
                    m_iMMSiSel--;
                    if (m_iMMSiSel < 0)
                    {
                        m_iMMSiSel = m_iTotal;
                    }
                    (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = "1.DESTINATION ID:" + m_strData[m_iMMSiSel, 3];
                }
                else if (m_iIntRogationSel == 2)
                {
                    if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                }
                else if (m_iIntRogationSel == 4)
                {
                    if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "NONE";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "NONE")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                }
                else if (m_iIntRogationSel == 6)
                {

                    m_iMMSiSel++;
                    if (m_iMMSiSel > m_iTotal)
                    {
                        m_iMMSiSel = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "2.DESTINATION ID:" + m_strData[m_iMMSiSel, 3];
                }
                else if (m_iIntRogationSel == 8)
                {
                    if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "NONE";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "NONE")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                }

            }
            else if (m_iFunOrder == 62)
            {
                if (m_iMainMenu_62 == 2)
                {
                    m_fGuardRNG-=0.1f;
                    if (m_fGuardRNG < 0)
                    {
                        m_fGuardRNG = 99;
                    }
                }
                else if (m_iMainMenu_62 == 5)
                {
                    if (m_fLostRNG > m_fGuardRNG + 1)
                    {
                        m_fLostRNG-=0.1f;
                    }
                    
                    if (m_fLostRNG < 0)
                    {
                        m_fLostRNG = 99;
                    }
                }
                Alarm_Setup();
            }
            else if (m_iFunOrder == 63)
            {
                if (m_iMainMenu_63 == 1)
                {
                    m_iContrast--;
                    if (m_iContrast < 0)
                    {
                        m_iContrast = 13;
                    }
                    (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = m_iContrast.ToString();
                }
                else if (m_iMainMenu_63 == 3)
                {
                    if (m_bUTC == false)
                    {
                        m_bUTC = true;
                        (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "OFF";
                    }
                    else
                    {
                        m_bUTC = false;
                        (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "ON";
                    }
                }
                else if (m_iMainMenu_63 == 7)
                {
                    if ((stackPanel_6_Mainmenu.Children[7] as TextBlock).Text == "AUTO")
                    {
                        (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "MANUAL";
                    }
                    else
                    {
                        (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "AUTO";
                    }
                }
                else if (m_iMainMenu_63 == 8)
                {
                    if (m_bSound == false)
                    {
                        m_bSound = true;
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.BUZZER         ：ON";
                    }
                    else
                    {
                        m_bSound = false;
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.BUZZER         ：OFF";
                    }
                }
            }
                if (m_iFunOrder == 4)//绘制高亮，主界面菜单
                {
                    if (m_iSelect > 0 && m_iSelect < 6)
                    {
                        m_iSelect -= 1;
                        Draw_Select();
                    }

                }
                else if (m_iFunOrder == 420)
                {
                    if (m_iOwnpos > 0)
                    {
                        m_iOwnpos--;
                        OwnposDisp_Select();
                    }
                }
                else if (m_iFunOrder == 411 && m_iList_Select > 3)
                {
                    m_iList_Select--;
                    Draw_Select();
                }
                else if (m_iFunOrder == 412 && m_iList_Select > 5)
                {
                    m_iList_Select--;
                    Draw_Select();
                }
                else if (m_iFunOrder == 413 && m_iList_Select > 8)
                {
                    m_iList_Select--;
                    Draw_Select();
                }
                else if (m_iFunOrder == 5)
                {
                    m_iKeyBoard_Sel--;
                    if (m_iKeyBoard_Sel == -1)
                    {
                        m_iKeyBoard_Sel = 65;

                    }

                    Sel_KeyBoard();

                }
                else if (m_iFunOrder == 7)
                {
                    m_iKeyBoard_Sel--;
                    if (m_iKeyBoard_Sel == -1)
                    {
                        m_iKeyBoard_Sel = 65;

                    }

                    Sel_KeyBoard();

                }
                else if (m_iFunOrder == 70)
                {
                    m_iKeyBoard_Sel--;
                    if (m_iKeyBoard_Sel == -1)
                    {
                        m_iKeyBoard_Sel = 65;

                    }

                    Sel_KeyBoard();

                }

                m_iQuickEdit = -1;
                m_QuickEdit.Start();
           
                Draw_My_Button(0, 8);//绘制按钮按下效果
            }
        
      
        private void Button_MouseLeftButtonUp_Stk_left(object sender, MouseButtonEventArgs e)//摇杆左键的左键抬起事件,8
        {
            m_QuickEdit.Stop();
            Draw_My_Button(1, 8);//绘制按钮抬起效果
        }
      
        private void Button_MouseLeftButtonDown_Stk_right(object sender, MouseButtonEventArgs e)//摇杆右键的左键按下事件,9
        {
            
            if(m_iFunOrder==20)
            {
                //RANGE设置
                if (m_iGrphic_Sel == 1)
                {
                    m_fDispRNG *= 2;
                    if (m_fDispRNG > 24)
                    {
                        m_fDispRNG = 0.75f;
                    }
                }
                    //BEARING设置
                else if (m_iGrphic_Sel == 3)
                {
                    if (m_iBRG_Disp == 0)
                    {
                        m_iBRG_Disp = 1;
                    }
                    else
                    {
                        m_iBRG_Disp = 0;
                    }
                }
                    //SORT设置
                else if (m_iGrphic_Sel == 5)
                {
                    m_iSORT_Disp++;
                    if (m_iSORT_Disp > 2)
                    {
                        m_iSORT_Disp = 0;
                    }              
                }
                    //GUARD RNG设置
                else if (m_iGrphic_Sel == 7)
                {
                    m_fGuardRNG+=0.1f;
                    if (m_fGuardRNG > 99)
                    {
                        m_fGuardRNG = 0;
                    }
                }
                    //NUMBER OF SHIPS DISP.设置
                else if (m_iGrphic_Sel == 9)
                {
                    
                    if (m_iNumOfShip == 128)
                    {
                        m_iNumOfShip = 22;
                    }
                    else if (m_iNumOfShip == 22)
                    {
                        m_iNumOfShip = 32;
                    }
                    else if (m_iNumOfShip == 32)
                    {
                        m_iNumOfShip = 64;
                    }
                    else if (m_iNumOfShip == 64)
                    {
                        m_iNumOfShip = 128;
                    }
                }
                    //CONTRAST设置
                else if (m_iGrphic_Sel == 11)
                {
                    m_iContrast++;
                    if (m_iContrast > 13)
                    {
                        m_iContrast = 0;
                    }
                }
                    //AUTO RNG设置
                else if (m_iGrphic_Sel == 13)
                {
                    if (m_bAutoRNG == false)
                    {
                        m_bAutoRNG = true;
                        m_fDispRNG = float.Parse(m_dRNG[m_iTotal-1].ToString("0.0"));
                    }
                    else
                    {
                        m_bAutoRNG = false;
                        m_fDispRNG = 1.5f;
                    }
                }
                Draw_Grphic_Setup();
            }
            if (m_iFunOrder == 60)
            {
                //NAV STATUS设置
                if (m_iMainMenu_60 == 1)
                {
                    m_iOwnNavStatus++;
                    if (m_iOwnNavStatus > 11)
                    {
                        m_iOwnNavStatus = 0;                      
                    }
                    NavStatus_Setup();
                }
                else if (m_iMainMenu_60 == 7)
                {
                    m_fOwnDraught+=0.1f;
                    if (m_fOwnDraught > 26)
                    {
                        m_fOwnDraught = 0;
                    }
                    m_strOwnshipData[16] = m_fOwnDraught.ToString("0.0");
                    (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = m_strOwnshipData[16] + "M";
                }
                else if (m_iMainMenu_60 == 9)
                {
                    m_iOwnCargoStatus++;
                    if (m_iOwnCargoStatus > 5)
                    {
                        m_iOwnCargoStatus = 0;
                    }
                    CargoStatus_Setup();
                }
                else if (m_iMainMenu_60 == 15)
                {
                    m_iPersons++;
                    if (m_iPersons > 50)
                    {
                        m_iPersons = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = m_iPersons.ToString() ;
                }
                else if (m_iMainMenu_60 == 17)
                {
                    m_iHeightKeel+=0.1f;
                    if (m_iHeightKeel > 100)
                    {
                        m_iHeightKeel = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = m_iHeightKeel.ToString("0.0")+"M";
                }

            
               
            }
            else if (m_iFunOrder == 613)
            {
                if (m_iIntRogationSel == 0)
                {
                    m_iMMSiSel++;
                    if (m_iMMSiSel > m_iTotal)
                    {
                        m_iMMSiSel = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[0] as TextBlock).Text = "1.DESTINATION ID:" + m_strData[m_iMMSiSel, 3];
                }
                else if (m_iIntRogationSel == 2)
                {
                    if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[2] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[2] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                }
                else if (m_iIntRogationSel == 4)
                {
                    if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "NONE")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[4] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[4] as TextBlock).Text = "NONE";
                    }
                }
                else if (m_iIntRogationSel == 6)
                {

                    m_iMMSiSel++;
                    if (m_iMMSiSel > m_iTotal)
                    {
                        m_iMMSiSel = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[6] as TextBlock).Text = "2.DESTINATION ID:" + m_strData[m_iMMSiSel, 3];
                }
                else if (m_iIntRogationSel == 8)
                {
                    if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "NONE")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "POSITION REPORT(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "POSITION REPORT(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SHIP STATIC AND VOYAGE(A)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SHIP STATIC AND VOYAGE(A)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SAR AIRCRAFT POS. REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SAR AIRCRAFT POS. REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "POSITION REPORT(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "POSITION REPORT(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "SHIP STATIC AND VOYAGE(B)";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "SHIP STATIC AND VOYAGE(B)")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "AIDS-TO-NAVIGATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "AIDS-TO-NAVIGATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "BASE STATION REPORT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "BASE STATION REPORT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "DATA LINK MANAGEMENT MSG";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "DATA LINK MANAGEMENT MSG")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "CHANNEL MENAGEMENT";
                    }
                    else if ((stackPanel_6_Mainmenu.Children[8] as TextBlock).Text == "CHANNEL MENAGEMENT")
                    {
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = "NONE";
                    }
                }

            }
            else if (m_iFunOrder == 603)
            {
                if (m_iETAChoose == 0)
                {
                    m_iMonth++;
                    if (m_iMonth > 12)
                    {
                        m_iMonth = 1;
                    }
                    ETA_Edit();
                }
                else if (m_iETAChoose == 1)
                {
                    m_iDay++;
                    ETA_Edit();
                }
                else if (m_iETAChoose == 2)
                {
                    m_iHour++;
                    if (m_iHour > 23)
                    {
                        m_iHour = 0;
                    }
                    (stackPanel_ETA_Edit.Children[2] as TextBlock).Text = m_iHour.ToString("00");
                }
                else if (m_iETAChoose == 3)
                {
                    m_iMinute++;
                    if (m_iMinute > 59)
                    {
                        m_iMinute = 0;
                    }
                    (stackPanel_ETA_Edit.Children[3] as TextBlock).Text = m_iMinute.ToString("00");
                }
            }
            else if (m_iFunOrder == 606)
            {
                if ((stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Text == " " + m_iWpChoose.ToString()+".")
                {
                    (stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Text = " " + (m_iWpChoose+1).ToString()+".";
                }
                else
                {
                    (stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Text = " " + m_iWpChoose.ToString() + ".";
                }
            }
            else if (m_iFunOrder == 6060)
            {
                if (m_iWpEditChoose < 16)
                {
                    m_iWpEditChoose++;
                    Draw_WayPointEdit();
                }
            }
            else if (m_iFunOrder == 610)
            {
                if (m_iTXSel == 0)
                {
                    if ((stackPanel_TX.Children[0] as TextBlock).Text == "1.FORMAT : " + "ADDRESSED")
                    {
                        (stackPanel_TX.Children[0] as TextBlock).Text = "1.FORMAT : " + "BROADCAST";
                    }
                    else
                    {
                        (stackPanel_TX.Children[0] as TextBlock).Text = "1.FORMAT : " + "ADDRESSED";
                    }
                }
                else if (m_iTXSel == 1)
                {
                    m_iMMSiSel++;
                    if (m_iMMSiSel > m_iTotal)
                    {
                        m_iMMSiSel = 0;
                    }
                    (stackPanel_TX.Children[1] as TextBlock).Text = "    MMSI : " + m_strData[m_iMMSiSel, 3];
                }
                else if (m_iTXSel == 2)
                {
                    if ((stackPanel_TX.Children[2] as TextBlock).Text == "2.CATAGORY : " + "ROUTINE")
                    {
                        (stackPanel_TX.Children[2] as TextBlock).Text = "2.CATAGORY : " + "SAFETY";
                    }
                    else
                    {
                        (stackPanel_TX.Children[2] as TextBlock).Text = "2.CATAGORY : " + "ROUTINE";
                    }
                }
                else if (m_iTXSel == 3)
                {
                    if ((stackPanel_TX.Children[4] as TextBlock).Text == "TEXT")
                    {
                        (stackPanel_TX.Children[4] as TextBlock).Text = "CAPABILITY INTERROGATE";
                    }
                    else
                    {
                        (stackPanel_TX.Children[4] as TextBlock).Text = "TEXT";
                    }
                }
                else if (m_iTXSel == 5)
                {
                    if ((stackPanel_TX.Children[5] as TextBlock).Text == "4.REPLY : " + "ON")
                    {
                        (stackPanel_TX.Children[5] as TextBlock).Text = "4.REPLY : " + "OFF";
                    }
                    else
                    {
                        (stackPanel_TX.Children[5] as TextBlock).Text = "4.REPLY : " + "ON";
                    }
                }
                else if (m_iTXSel == 7)
                {
                    if ((stackPanel_TX.Children[7] as TextBlock).Text == "5.CH : " + "A/B")
                    {
                        (stackPanel_TX.Children[7] as TextBlock).Text = "5.CH : " + "A";

                    }
                    else if ((stackPanel_TX.Children[7] as TextBlock).Text == "5.CH : " + "A")
                    {
                        (stackPanel_TX.Children[7] as TextBlock).Text = "5.CH : " + "B";
                    }
                    else if ((stackPanel_TX.Children[7] as TextBlock).Text == "5.CH : " + "B")
                    {
                        (stackPanel_TX.Children[7] as TextBlock).Text = "5.CH : " + "AUTO";
                    }
                    else
                    {
                        (stackPanel_TX.Children[7] as TextBlock).Text = "5.CH : " + "A/B";
                    }
                }
                else if (m_iTXSel == 9)
                {
                    if ((stackPanel_TX.Children[9] as TextBlock).Text == "6.NUMBER OF RETRY : " + "3")
                    {
                        (stackPanel_TX.Children[9] as TextBlock).Text = "6.NUMBER OF RETRY : " + "0";
                    }
                    else
                    {
                        (stackPanel_TX.Children[9] as TextBlock).Text = "6.NUMBER OF RETRY : " + "3";
                    }
                }
            }
            else if (m_iFunOrder == 62)
            {
                if (m_iMainMenu_62 == 2)
                {
                    m_fGuardRNG+=0.1f;
                    if (m_fGuardRNG > 99)
                    {
                        m_fGuardRNG = 0;
                    }
                }
                else if (m_iMainMenu_62 == 5)
                {

                    m_fLostRNG+=0.1f;

                    if (m_fLostRNG > 99)
                    {
                        m_fLostRNG = m_fGuardRNG + 0.1f;
                    }
                }
                Alarm_Setup();
            }
            else if (m_iFunOrder == 63)
            {
                if (m_iMainMenu_63 == 1)
                {
                    m_iContrast++;
                    if (m_iContrast > 13)
                    {
                        m_iContrast = 0;
                    }
                    (stackPanel_6_Mainmenu.Children[1] as TextBlock).Text = m_iContrast.ToString();
                }
                else if (m_iMainMenu_63 == 3)
                {
                    if (m_bUTC == false)
                    {
                        m_bUTC = true;
                        (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "OFF";
                    }
                    else
                    {
                        m_bUTC = false;
                        (stackPanel_6_Mainmenu.Children[3] as TextBlock).Text = "ON";
                    }
                }
                else if (m_iMainMenu_63 == 7)
                {
                    if ((stackPanel_6_Mainmenu.Children[7] as TextBlock).Text == "AUTO")
                    {
                        (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "MANUAL";
                    }
                    else
                    {
                        (stackPanel_6_Mainmenu.Children[7] as TextBlock).Text = "AUTO";
                    }
                }
                else if (m_iMainMenu_63 == 8)
                {
                    if (m_bSound == false)
                    {
                        m_bSound = true;
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.BUZZER         ：ON";
                    }
                    else
                    {
                        m_bSound = false;
                        (stackPanel_6_Mainmenu.Children[8] as TextBlock).Text = " 5.BUZZER         ：OFF";
                    }
                }
            }
            else if (m_iFunOrder == 4)//绘制高亮，主界面菜单
            {
                if (m_iSelect > -1 && m_iSelect < 5)
                {
                    m_iSelect += 1;
                    Draw_Select();
                }

            }
            else if (m_iFunOrder == 420)
            {
                if (m_iOwnpos < 1)
                {
                    m_iOwnpos++;
                    OwnposDisp_Select();
                }
            }
            else if (m_iFunOrder == 411 && m_iList_Select < 4)
            {
                m_iList_Select++;
                Draw_Select();
            }
            else if (m_iFunOrder == 412 && m_iList_Select < 7)
            {
                m_iList_Select++;
                Draw_Select();
            }
            else if (m_iFunOrder == 413 && m_iList_Select < 9)
            {
                m_iList_Select++;
                Draw_Select();
            }
            else if (m_iFunOrder == 5)
            {
                m_iKeyBoard_Sel++;
                if (m_iKeyBoard_Sel == 66)
                {
                    m_iKeyBoard_Sel = 0;

                }

                Sel_KeyBoard();

            }
            else if (m_iFunOrder == 7)
            {
                m_iKeyBoard_Sel++;


                Sel_KeyBoard();


            }
            else if (m_iFunOrder == 70)
            {
                m_iKeyBoard_Sel++;


                Sel_KeyBoard();


            }
            m_iQuickEdit = 1;
            m_QuickEdit.Start();
            Draw_My_Button(0, 9);//绘制按钮按下效果
        }
      
        private void Button_MouseLeftButtonUp_Stk_right(object sender, MouseButtonEventArgs e)//摇杆右键的左键抬起事件,9
        {
            m_QuickEdit.Stop();
            Draw_My_Button(1, 9);//绘制按钮抬起效果
        }
      
        private void Button_MouseLeftButtonDown_Stk(object sender, MouseButtonEventArgs e)//摇杆按键的左键按下事件,10
        {
            Draw_My_Button(0,10);//绘制按钮按下效果
            if (m_iFunOrder == 1 && rectangle_ChoseSign_Top.Visibility == Visibility.Collapsed)
            {
                                        
                m_iFunOrder = 3;
                TitleDisplay(9);
                Draw_My_Screen(m_Struct_No1);//绘制界面               
                Data_Display();
           
            }
            else if (m_iFunOrder == 1 && rectangle_ChoseSign_Top.Visibility == Visibility.Visible)
            {

                m_iFunOrder = 43;

                TitleDisplay(8);
                Draw_My_Screen(m_Struct_No1);//绘制界面               
                Data_Display();

            }
            else if (m_iFunOrder == 2)
            {
                if(rectangle_ChoseSign_Top.Visibility==Visibility.Collapsed)
                {
                m_iFunOrder = 20;
                warpPanel_2_Setup.Visibility = Visibility.Visible;
                stackPanel_2_Stakpnl_Setup.Visibility = Visibility.Collapsed;
                rectangle_2_Stakpnl_Setup.Visibility = Visibility.Collapsed;
                canvas_2_Map.Visibility = Visibility.Collapsed;
                canvas_2_Ship.Visibility = Visibility.Collapsed;
                rec_2_Down.Visibility = Visibility.Visible;
                TitleDisplay(11);
                Draw_Grphic_Setup();
                }
            
            }
            else if (m_iFunOrder == 3)
            {
                if (m_Detail_Choose == 2)
                {
                    m_Detail_Choose = 0;
                    m_iFunOrder = 1;
                    TitleDisplay(m_iTitle_Choose);
                    Draw_My_Screen(m_Struct_No1);//绘制界面
                    Data_Display();
                }
                else if (m_Detail_Choose == 3)
                {
                    m_Detail_Choose = 0;
                    m_iFunOrder = 610;
                    m_iMMSiSel = m_Count_Choose;
                    MainMenu_Change();
                    Draw_TX();

                    Grid_3.Visibility = Visibility.Collapsed;
                    Grid_DetailSel.Visibility = Visibility.Collapsed;
                }
                else if (m_Detail_Choose == 4)
                {
                    textBlock_Bottom.Text = " ACK1:OK         ACK2:OK";
                }
            }
            else if (m_iFunOrder == 6)
            {
                if (m_iMainMenu_Sel == 0)
                {
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_Sel == 2)
                {
                    m_iFunOrder = 61;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_Sel == 4)
                {
                    m_iFunOrder = 62;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_Sel == 6)
                {
                    m_iMainMenu_63 = 0;
                    m_iFunOrder = 63;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_Sel == 8)
                {
                    m_iFunOrder = 64;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 64)
            {
                if (m_iMainMenu_64 == 1)
                {
                    m_iFunOrder = 640;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_64 == 3)
                {
                    m_iFunOrder = 641;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_64 == 5)
                {
                    m_iFunOrder = 642;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_64 == 7)
                {
                    m_iFunOrder = 643;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_64 == 9)
                {
                    m_iFunOrder = 644;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_64 == 11)
                {
                    m_iFunOrder = 645;
                    MainMenu_Change();
                }
            }
            else if (m_iFunOrder == 61)
            {
                if (m_iMainMenu_61 == 1)
                {
                    m_iFunOrder = 610;
                    MainMenu_Change();
                    Draw_TX();
                }
                else if (m_iMainMenu_61 == 3)
                {
                    m_iFunOrder = 611;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_61 == 5)
                {
                    m_iFunOrder = 612;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_61 == 7)
                {
                    m_iFunOrder = 613;
                    MainMenu_Change();
                    Draw_IntRogation();
                }
                else if (m_iMainMenu_61 == 9)
                {
                    m_iFunOrder = 614;
                    MainMenu_Change();
                }
            }
            else if (m_iFunOrder == 63)
            {
                if (m_iMainMenu_63 < 13)
                {
                    if (m_iMainMenu_63 % 2 == 0 && m_iMainMenu_63 != 4 && m_iMainMenu_63 != 8 && m_iMainMenu_63 != 10 && m_iMainMenu_63 != 12)
                    {
                        m_iMainMenu_63++;
                        Draw_MainMenu();
                    }
                    else if (m_iMainMenu_63 % 2 != 0)
                    {
                        m_iMainMenu_63++;
                        Draw_MainMenu();
                    }
                }
            }
            else if (m_iFunOrder == 60)
            {
                if (m_iMainMenu_60 < 9)
                {
                    if (m_iMainMenu_60 % 2 == 0 && m_iMainMenu_60 != 2&&m_iMainMenu_60!=4)
                    {
                        m_iMainMenu_60++;
                        Draw_MainMenu();
                    }
                    else if (m_iMainMenu_60 % 2 != 0)
                    {
                        m_iMainMenu_60++;
                        Draw_MainMenu();
                    }
                    else if (m_iMainMenu_60 == 2)
                    {
                        m_iFunOrder = 7;
                        Draw_My_Screen(m_Struct_No1);
                        Data_Display();
                        m_iKeyBoard_Sel = 0;
                        Sel_KeyBoard();
                    }
                    else if (m_iMainMenu_60 == 4)
                    {
                        m_iFunOrder = 603;
                        MainMenu_Change();
                        Draw_ETA();
                    }
                }
                else if (m_iMainMenu_60 == 9)
                {
                    m_iMainMenu_60 = 10;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 == 10)
                {
                    m_iFunOrder = 606;
                    MainMenu_Change();
                    Draw_WayPoint();
                }
                else if (m_iMainMenu_60 == 14 || m_iMainMenu_60 == 15 || m_iMainMenu_60 == 16)
                {
                    m_iMainMenu_60++;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 == 18)
                {
                    m_iFunOrder = 6;
                    MainMenu_Change();
                    Draw_MainMenu();
                    m_iOwnNavStatus = 0;
                    m_iOwnCargoStatus = 0;
                    m_fOwnDraught = 5;
                    m_strOwnshipData[16] = m_fOwnDraught.ToString("0.0");
                    m_iPersons = 16;
                    m_iHeightKeel = 30;
                    m_strOwnshipData[11] = "DALIAN";
                    m_strOwnshipData[12] = "16/APR";
                    m_strOwnshipData[13] = "14:00";
                }
                else if (m_iMainMenu_60 == 20)
                {
                    m_iFunOrder = 6;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 == 22)
                {
                   
                }
                

            }
            else if (m_iFunOrder == 603)
            {
                if (m_iETAChoose < 3)
                {
                    m_iETAChoose++;
                    Draw_ETA();
                }
                else
                {
                    Save_ETA();
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 606)
            {
                if (m_iWpChoose < 6)
                {
                    if ((stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Text == "CLR")
                    {
                        m_WpLAT[m_iWpChoose - 1] = 0;
                        m_WpLON[m_iWpChoose - 1] = 0;
                        MainMenu_Change();
                    }
                    else if ((stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Text == " " + (m_iWpChoose + 1).ToString() + ".")
                    {
                        if ((stackPanel_Wp_Pos.Children[9] as TextBlock).Text == "")
                        {
                            try
                            {
                                m_WpLAT[m_iWpChoose + 3] = m_WpLAT[m_iWpChoose + 2];
                                m_WpLON[m_iWpChoose + 3] = m_WpLON[m_iWpChoose + 2];
                                m_WpLAT[m_iWpChoose + 2] = m_WpLAT[m_iWpChoose + 1];
                                m_WpLON[m_iWpChoose + 2] = m_WpLON[m_iWpChoose + 1];
                                m_WpLAT[m_iWpChoose + 1] = m_WpLAT[m_iWpChoose];
                                m_WpLON[m_iWpChoose + 1] = m_WpLON[m_iWpChoose];
                                m_WpLAT[m_iWpChoose] = 0;
                                m_WpLON[m_iWpChoose] = 0;
                                MainMenu_Change();
                            }
                            catch
                            {
                            }
                        }
                    }
                    else
                    {
                        m_iFunOrder = 6060;
                        MainMenu_Change();
                        Draw_WayPointEdit();
                    }
                }
                else if (m_iWpChoose == 6)
                {
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                    for (int i = 0; i < 6; i++)
                    {
                        m_WpLAT[i] = 0;
                        m_WpLON[i] = 0;
                    }
                }
                else if (m_iWpChoose == 7)
                {
                }
                else if (m_iWpChoose == 8)
                {
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iWpChoose == 9)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        m_WpLAT[i] = 0;
                        m_WpLON[i] = 0;
                    }
                    MainMenu_Change();
                }
                else if (m_iWpChoose == 10)
                {

                }
            }
            else if (m_iFunOrder == 6060)
            {
                if (m_iWpEditChoose < 16)
                {
                    m_iWpEditChoose++;
                    Draw_WayPointEdit();
                }
                else
                {
                    WayPointSave();
                    m_iFunOrder = 606;
                    MainMenu_Change();
                }
            }
            else if (m_iFunOrder == 611)
            {
                m_iFunOrder = 6111;
                MainMenu_Change();
            }
            else if (m_iFunOrder == 612)
            {
                m_iFunOrder = 6121;
                MainMenu_Change();
            }
            else if (m_iFunOrder == 613)
            {
                if (m_iIntRogationSel == 10)
                {
                    m_iFunOrder = 61;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iIntRogationSel == 12)
                {
                    textBlock_Bottom.Text = " ACK1:           ACK2:";
                }
                else if (m_iIntRogationSel == 14)
                {
                    m_iIntRogationSel = 0;
                    m_iMMSiSel = 0;
                    Draw_IntRogation();
                }
                else if (m_iIntRogationSel == 16)
                {
                    textBlock_Bottom.Text = " ACK1:OK         ACK2:";
                }
                else if (m_iIntRogationSel == 18)
                {
                    textBlock_Bottom.Text = " ACK1:OK         ACK2:";
                }
                else if (m_iIntRogationSel == 20)
                {
                    if (textBlock_Bottom.Text == " ACK1:OK         ACK2:")
                    {
                        textBlock_Bottom.Text = " ACK1:OK         ACK2:OK";
                    }
                    else
                    {
                        textBlock_Bottom.Text = " ACK1:         ACK2:OK";
                    }
                }
            }
            else if (m_iFunOrder == 62)
            {
                if (m_iMainMenu_62 == 1 || m_iMainMenu_62 == 4)
                {
                    m_iMainMenu_62++;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_62 == 2 || m_iMainMenu_62 == 5)
                {
                    m_iMainMenu_62 += 2;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_62 == 7)
                {
                    m_iFunOrder = 622;
                    MainMenu_Change();
                }
            }
            else if (m_iFunOrder == 20)
            {
                if (m_iGrphic_Sel < 14)
                {
                    if (m_iGrphic_Sel % 2 == 0)
                    {
                        m_iGrphic_Sel++;
                        Draw_Grphic_Setup();
                    }
                    else
                    {
                        m_iGrphic_Sel++;
                        Draw_Grphic_Setup();
                    }
                }
                //选择EXIT
                else if (m_iGrphic_Sel == 14)
                {
                    //所有参数恢复默认值
                    m_fDispRNG = 1.5f;
                    m_iBRG_Disp = 0;
                    m_iSORT_Disp = 0;
                    m_fGuardRNG = 12;
                    m_iNumOfShip = 22;
                    m_iContrast = 7;
                    m_bAutoRNG = false;
                    m_iFunOrder = 2;
                    Draw_My_Screen(m_Struct_No1);//绘制界面
                    rectangle_ChoseSign_Top.Visibility = Visibility.Visible;
                    Data_Display();
                    m_iGrphic_Sel = 0;
                    RealTime_Disp();
                }
                //选择ENT
                else if (m_iGrphic_Sel == 16)
                {
                    m_iFunOrder = 2;
                    Draw_My_Screen(m_Struct_No1);//绘制界面
                    Data_Display();
                    rectangle_ChoseSign_Top.Visibility = Visibility.Visible;
                    m_iGrphic_Sel = 0;
                    RealTime_Disp();
                }


            }
            else if (m_iFunOrder == 3)
            {
                m_Detail_Choose = 0;
                TitleDisplay(m_iTitle_Choose);
                m_iFunOrder = 1;
                Draw_My_Screen(m_Struct_No1);//绘制界面

            }
            else if (m_iFunOrder == 4)
            {
                if (m_iSelect == 0)
                {
                    m_iFunOrder = 1;
                    Draw_My_Screen(m_Struct_No1);//绘制界面
                    Draw_Choose(0);
                    Data_Display();
                }
                else if (m_iSelect == 1)
                {
                    m_iFunOrder = 41;
                    m_iList_Select = 0;
                    grid_4_Down_List.Visibility = Visibility.Visible;

                    warpPanel_4_Down.Visibility = Visibility.Collapsed;
                    grid_4_Down.Visibility = Visibility.Collapsed;
                    Draw_Select();
                }
                else if (m_iSelect == 2)
                {
                    m_iFunOrder = 420;
                    warpPanel_4_Down_Owndisp.Visibility = Visibility.Visible;
                    grid_4_Down_Ownship.Visibility = Visibility.Visible;
                    warpPanel_4_Down.Visibility = Visibility.Collapsed;
                    grid_4_Down.Visibility = Visibility.Collapsed;
                    OwnposDisp_Select();


                }
                else if (m_iSelect == 3)
                {

                    m_iFunOrder = 43;

                    TitleDisplay(8);
                    Draw_My_Screen(m_Struct_No1);//绘制界面               
                    Data_Display();

                }
                else if (m_iSelect == 4 || m_iSelect == 5)
                {
                    Data_Display();

                }



            }
            else if (m_iFunOrder == 420)
            {
                if (m_iOwnpos == 1)
                {
                    m_bOwnposdisp = false;
                    m_iFunOrder = 4;
                    m_Count_Choose_Down = 15;
                    m_Count_Choose_Up = 0;
                    m_iSelect = 0;
                    Draw_Select();

                    polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                    Draw_My_Screen(m_Struct_No1);//绘制界面               
                    Data_Display();

                }
                else if (m_iOwnpos == 0)
                {
                    m_bOwnposdisp = true;
                    m_iFunOrder = 4;
                    m_iSelect = 0;

                    m_Count_Choose_Down = 13;
                    m_Count_Choose_Up = 0;
                    Draw_Select();
                    polygon_ChoseSign_Down_Copy.Visibility = Visibility.Visible;

                    Draw_My_Screen(m_Struct_No1);//绘制界面               
                    Data_Display();
                }





            }
            else if (m_iFunOrder == 43)//数据翻页，本船详细信息界面
            {
                m_iFunOrder = 4;
                m_iSelect = 0;
                m_Detail_Choose = 0;
                Draw_My_Screen(m_Struct_No1);
                Data_Display();
                Draw_Select();
                TitleDisplay(m_iTitle_Choose);

            }
            else if (m_iFunOrder == 41)//List菜单界面
            {
                if (m_iList_Select == 0)
                {
                    m_iList_Select = 3;
                    m_iFunOrder = 411;
                    Draw_Select();
                }
                else if (m_iList_Select == 1)
                {
                    m_iList_Select = 5;
                    m_iFunOrder = 412;
                    Draw_Select();
                }
                else if (m_iList_Select == 2)
                {
                    m_iList_Select = 8;
                    m_iFunOrder = 413;
                    Draw_Select();
                }

            }
            else if (m_iFunOrder == 411)//List菜单界面
            {
                if (m_iList_Select == 3)
                {
                    m_iBRG_Disp = 1;
                    if (m_iSORT_Disp == 0)
                    {
                        m_iTitle_Choose = 2;
                        TitleDisplay(m_iTitle_Choose);

                    }
                    else if (m_iSORT_Disp == 1)
                    {
                        m_iTitle_Choose = 4;
                        TitleDisplay(m_iTitle_Choose);
                    }
                    else if (m_iSORT_Disp == 2)
                    {
                        m_iTitle_Choose = 6;
                        TitleDisplay(m_iTitle_Choose);
                    }
                }
                else if (m_iList_Select == 4)
                {
                    m_iBRG_Disp = 0;
                    if (m_iSORT_Disp == 0)
                    {
                        m_iTitle_Choose = 1;
                        TitleDisplay(m_iTitle_Choose);

                    }
                    else if (m_iSORT_Disp == 1)
                    {
                        m_iTitle_Choose = 3;
                        TitleDisplay(m_iTitle_Choose);
                    }
                    else if (m_iSORT_Disp == 2)
                    {
                        m_iTitle_Choose = 5;
                        TitleDisplay(m_iTitle_Choose);
                    }
                }
            }
            else if (m_iFunOrder == 412)//List菜单界面
            {
                if (m_iList_Select == 5)
                {
                    m_iSORT_Disp = 0;
                    if (m_iBRG_Disp == 0)
                    {
                        m_iTitle_Choose = 1;
                        TitleDisplay(m_iTitle_Choose);

                    }
                    else if (m_iBRG_Disp == 1)
                    {
                        m_iTitle_Choose = 2;
                        TitleDisplay(m_iTitle_Choose);
                    }
                }
                else if (m_iList_Select == 6)
                {
                    m_iSORT_Disp = 1;
                    if (m_iBRG_Disp == 0)
                    {
                        m_iTitle_Choose = 3;
                        TitleDisplay(m_iTitle_Choose);

                    }
                    else if (m_iBRG_Disp == 1)
                    {
                        m_iTitle_Choose = 4;
                        TitleDisplay(m_iTitle_Choose);
                    }
                }
                else if (m_iList_Select == 7)
                {
                    m_iSORT_Disp = 2;
                    if (m_iBRG_Disp == 0)
                    {
                        m_iTitle_Choose = 5;
                        TitleDisplay(m_iTitle_Choose);

                    }
                    else if (m_iBRG_Disp == 1)
                    {
                        m_iTitle_Choose = 6;
                        TitleDisplay(m_iTitle_Choose);
                    }
                }


            }

            else if (m_iFunOrder == 413)//List菜单界面
            {
                if (m_iList_Select == 8)
                {
                    m_iNAME_Disp = 0;
                    Data_Display();

                }
                if (m_iList_Select == 9)
                {
                    m_iNAME_Disp = 1;
                    Data_Display();
                }
            }
            else if (m_iFunOrder == 5)//输入密码关机
            {
                if (m_iPassword_sel < 4)
                {
                    m_iPassword_sel++;
                    Draw_Password();

                }
                if (m_strPassCodeinPut.Length < 4)//已输入密码记录
                {
                    m_strPassCodeinPut += (warpPanel_5_Input.Children[m_iKeyBoard_Sel] as TextBlock).Text;
                }
            }
            else if (m_iFunOrder == 7)
            {
                if (m_iKeyBoard_Sel < 66)
                {
                    textBlock_InputText.Text += (warpPanel_Input.Children[m_iKeyBoard_Sel] as TextBlock).Text;
                }
                else if (m_iKeyBoard_Sel == 66)
                {
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                    Grid_Input.Visibility = Visibility.Collapsed;
                    Grid_6.Visibility = Visibility.Visible;
                }
                else if (m_iKeyBoard_Sel == 67)
                {
                    m_strOwnshipData[11] = textBlock_InputText.Text;
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                    Grid_Input.Visibility = Visibility.Collapsed;
                    Grid_6.Visibility = Visibility.Visible;
                }
                else if (m_iKeyBoard_Sel == 68)
                {
                    textBlock_InputText.Text = "";
                }

            }
            else if (m_iFunOrder == 70)
            {
                if (m_iKeyBoard_Sel < 66)
                {
                    textBlock_InputText.Text += (warpPanel_Input.Children[m_iKeyBoard_Sel] as TextBlock).Text;
                }
                else if (m_iKeyBoard_Sel == 66)
                {
                    m_iFunOrder = 61;
                    MainMenu_Change();
                    Draw_MainMenu();
                    Grid_TX.Visibility = Visibility.Collapsed;
                    Grid_Input.Visibility = Visibility.Collapsed;
                    Grid_6.Visibility = Visibility.Visible;
                }
                else if (m_iKeyBoard_Sel == 67)
                {
                    Save_Message();
                    m_iFunOrder = 61;
                    MainMenu_Change();
                    Draw_MainMenu();
                    Grid_TX.Visibility = Visibility.Collapsed;
                    Grid_Input.Visibility = Visibility.Collapsed;
                    Grid_6.Visibility = Visibility.Visible;
                }

            }
            if (m_strPassCode == m_strPassCodeinPut)
            {
           
               m_bPower = true;                           
            }

        

        
   

           
        }
      
        private void Button_MouseLeftButtonUp_Stk(object sender, MouseButtonEventArgs e)//摇杆按键的左键抬起事件,10
        {
            Draw_My_Button(1,10);//绘制按钮抬起效果
        }

        private void Btn_MouseLeftButtonDown_Enter(object sender, MouseButtonEventArgs e)
        {
            Button_Sound();
            Draw_My_Button(0,12);
            if (m_iFunOrder == 1 && rectangle_ChoseSign_Top.Visibility == Visibility.Collapsed)
            {

                m_iFunOrder = 3;
                TitleDisplay(9);
                Draw_My_Screen(m_Struct_No1);//绘制界面               
                Data_Display();

            }
            else if (m_iFunOrder == 1 && rectangle_ChoseSign_Top.Visibility == Visibility.Visible)
            {

                m_iFunOrder = 43;
                
                TitleDisplay(8);
                Draw_My_Screen(m_Struct_No1);//绘制界面               
                Data_Display();

            }
            else if (m_iFunOrder == 2)
            {
                if (rectangle_ChoseSign_Top.Visibility == Visibility.Collapsed)
                {
                    m_iFunOrder = 20;
                    warpPanel_2_Setup.Visibility = Visibility.Visible;
                    stackPanel_2_Stakpnl_Setup.Visibility = Visibility.Collapsed;
                    rectangle_2_Stakpnl_Setup.Visibility = Visibility.Collapsed;
                    canvas_2_Map.Visibility = Visibility.Collapsed;
                    canvas_2_Ship.Visibility = Visibility.Collapsed;
                    rec_2_Down.Visibility = Visibility.Visible;
                    TitleDisplay(11);
                    Draw_Grphic_Setup();
                }

            }
            else if (m_iFunOrder == 3)
            {
                if (m_Detail_Choose == 2)
                {
                    m_Detail_Choose = 0;
                    m_iFunOrder = 1;
                    TitleDisplay(m_iTitle_Choose);
                    Draw_My_Screen(m_Struct_No1);//绘制界面
                    Data_Display();
                }
                else if (m_Detail_Choose == 3)
                {
                    m_Detail_Choose = 0;
                    m_iFunOrder = 610;
                    m_iMMSiSel = m_Count_Choose;
                    MainMenu_Change();
                    Draw_TX();
                  
                    Grid_3.Visibility = Visibility.Collapsed;
                    Grid_DetailSel.Visibility = Visibility.Collapsed;
                }
                else if (m_Detail_Choose == 4)
                {
                    textBlock_Bottom.Text = " ACK1:OK         ACK2:OK";
                }
            }
            else if (m_iFunOrder == 6)
            {
                if (m_iMainMenu_Sel == 0)
                {
                    m_iFunOrder = 60;
                    m_iMainMenu_60 = 0;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_Sel == 2)
                {
                    m_iFunOrder = 61;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_Sel == 4)
                {
                    m_iFunOrder = 62;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_Sel == 6)
                {
                    m_iFunOrder = 63;
                    m_iMainMenu_63 = 0;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_Sel == 8)
                {
                    m_iFunOrder = 64;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 64)
            {
                if (m_iMainMenu_64 == 1)
                {
                    m_iFunOrder = 640;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_64 == 3)
                {
                    m_iFunOrder = 641;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_64 == 5)
                {
                    m_iFunOrder = 642;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_64 == 7)
                {
                    m_iFunOrder = 643;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_64 == 9)
                {
                    m_iFunOrder = 644;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_64 == 11)
                {
                    m_iFunOrder = 645;
                    MainMenu_Change();
                }
            }
            else if (m_iFunOrder == 63)
            {
                if (m_iMainMenu_63 < 13)
                {
                    if (m_iMainMenu_63 % 2 == 0 && m_iMainMenu_63 != 4 && m_iMainMenu_63 != 8 && m_iMainMenu_63 != 10 && m_iMainMenu_63 != 12)
                    {
                        m_iMainMenu_63++;
                        Draw_MainMenu();
                    }
                    else if (m_iMainMenu_63 % 2 != 0)
                    {
                        m_iMainMenu_63++;
                        Draw_MainMenu();
                    }
                }
            }
            else if (m_iFunOrder == 61)
            {
                if (m_iMainMenu_61 == 1)
                {
                    m_iFunOrder = 610;
                    MainMenu_Change();
                    Draw_TX();
                }
                else if (m_iMainMenu_61 == 3)
                {
                    m_iFunOrder = 611;
                    MainMenu_Change();
                }
                else if (m_iMainMenu_61 == 5)
                {
                    m_iFunOrder = 612;
                    MainMenu_Change();
                }

                else if (m_iMainMenu_61 == 7)
                {
                    m_iFunOrder = 613;
                    MainMenu_Change();
                    Draw_IntRogation();
                }
                else if (m_iMainMenu_61 == 9)
                {
                    m_iFunOrder = 614;
                    MainMenu_Change();
                }
            }
            else if (m_iFunOrder == 611)
            {
                m_iFunOrder = 6111;
                MainMenu_Change();
            }
            else if (m_iFunOrder == 612)
            {
                m_iFunOrder = 6121;
                MainMenu_Change();
            }
            else if (m_iFunOrder == 613)
            {
                if (m_iIntRogationSel == 10)
                {
                    m_iFunOrder = 61;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iIntRogationSel == 12)
                {
                    textBlock_Bottom.Text = " ACK1:           ACK2:";
                }
                else if (m_iIntRogationSel == 14)
                {
                    m_iIntRogationSel = 0;
                    m_iMMSiSel = 0;
                    Draw_IntRogation();
                }
                else if (m_iIntRogationSel == 16)
                {
                    textBlock_Bottom.Text = " ACK1:OK         ACK2:";
                }
                else if (m_iIntRogationSel == 18)
                {
                    textBlock_Bottom.Text = " ACK1:OK         ACK2:";
                }
                else if (m_iIntRogationSel == 20)
                {
                    if (textBlock_Bottom.Text == " ACK1:OK         ACK2:")
                    {
                        textBlock_Bottom.Text = " ACK1:OK         ACK2:OK";
                    }
                    else
                    {
                        textBlock_Bottom.Text = " ACK1:         ACK2:OK";
                    }
                }
            }
            else if (m_iFunOrder == 60)
            {
                if (m_iMainMenu_60 < 9)
                {
                    if (m_iMainMenu_60 % 2 == 0 && m_iMainMenu_60 != 2 && m_iMainMenu_60 != 4)
                    {
                        m_iMainMenu_60++;
                        Draw_MainMenu();
                    }
                    else if (m_iMainMenu_60 % 2 != 0)
                    {
                        m_iMainMenu_60++;
                        Draw_MainMenu();
                    }
                    else if (m_iMainMenu_60 == 2)
                    {
                        m_iFunOrder = 7;
                        Draw_My_Screen(m_Struct_No1);
                        Data_Display();
                        m_iKeyBoard_Sel = 0;
                        Sel_KeyBoard();
                    }
                    else if (m_iMainMenu_60 == 4)
                    {
                        m_iFunOrder = 603;
                        MainMenu_Change();
                        Draw_ETA();
                    }
                }
                else if (m_iMainMenu_60 == 9)
                {
                    m_iMainMenu_60 = 10;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 == 10)
                {
                    m_iFunOrder = 606;
                    MainMenu_Change();
                    Draw_WayPoint();
                }
                else if (m_iMainMenu_60 == 14 || m_iMainMenu_60 == 15 || m_iMainMenu_60 == 16)
                {
                    m_iMainMenu_60++;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 == 18)
                {
                    m_iFunOrder = 6;
                    MainMenu_Change();
                    Draw_MainMenu();
                    m_iOwnNavStatus = 0;
                    m_iOwnCargoStatus = 0;
                    m_fOwnDraught = 5;
                    m_strOwnshipData[16] = m_fOwnDraught.ToString("0.0");
                    m_iPersons = 16;
                    m_iHeightKeel = 30;
                    m_strOwnshipData[11] = "DALIAN";
                    m_strOwnshipData[12] = "16/APR";
                    m_strOwnshipData[13] = "14:00";
                }
                else if (m_iMainMenu_60 == 20)
                {
                    m_iFunOrder = 6;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_60 == 22)
                {

                }
            }
            else if (m_iFunOrder == 603)
            {
                if (m_iETAChoose < 3)
                {
                    m_iETAChoose++;
                    Draw_ETA();
                }
                else
                {
                    Save_ETA();
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
            }
            else if (m_iFunOrder == 606)
            {
                if (m_iWpChoose < 6)
                {
                    if ((stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Text == "CLR")
                    {
                        m_WpLAT[m_iWpChoose - 1] = 0;
                        m_WpLON[m_iWpChoose - 1] = 0;
                        MainMenu_Change();
                    }
                    else if ((stackPanel_Wp_Num.Children[m_iWpChoose] as TextBlock).Text == " " + (m_iWpChoose + 1).ToString() + ".")
                    {
                        if ((stackPanel_Wp_Pos.Children[9] as TextBlock).Text == "")
                        {
                            try
                            {
                                m_WpLAT[m_iWpChoose + 3] = m_WpLAT[m_iWpChoose + 2];
                                m_WpLON[m_iWpChoose + 3] = m_WpLON[m_iWpChoose + 2];
                                m_WpLAT[m_iWpChoose + 2] = m_WpLAT[m_iWpChoose + 1];
                                m_WpLON[m_iWpChoose + 2] = m_WpLON[m_iWpChoose + 1];
                                m_WpLAT[m_iWpChoose + 1] = m_WpLAT[m_iWpChoose];
                                m_WpLON[m_iWpChoose + 1] = m_WpLON[m_iWpChoose];
                                m_WpLAT[m_iWpChoose] = 0;
                                m_WpLON[m_iWpChoose] = 0;
                                MainMenu_Change();
                            }
                            catch
                            {
                            }
                        }
                    }
                    else
                    {
                        m_iFunOrder = 6060;
                        MainMenu_Change();
                        Draw_WayPointEdit();
                    }
                }
                else if (m_iWpChoose == 6)
                {
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                    for (int i = 0; i < 6; i++)
                    {
                        m_WpLAT[i] = 0;
                        m_WpLON[i] = 0;
                    }
                }
                else if (m_iWpChoose == 7)
                {
                }
                else if (m_iWpChoose == 8)
                {
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                }
                else if (m_iWpChoose == 9)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        m_WpLAT[i] = 0;
                        m_WpLON[i] = 0;
                    }
                    MainMenu_Change();
                }
                else if (m_iWpChoose == 10)
                {

                }
            }

            else if (m_iFunOrder == 6060)
            {
                if (m_iWpEditChoose < 16)
                {
                    m_iWpEditChoose++;
                    Draw_WayPointEdit();
                }
                else
                {
                    WayPointSave();
                    m_iFunOrder = 606;
                    MainMenu_Change();
                }
            }
            else if (m_iFunOrder == 62)
            {
                if (m_iMainMenu_62 == 1 || m_iMainMenu_62 == 4)
                {
                    m_iMainMenu_62++;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_62 == 2 || m_iMainMenu_62 == 5)
                {
                    m_iMainMenu_62 += 2;
                    Draw_MainMenu();
                }
                else if (m_iMainMenu_62 == 7)
                {
                    m_iFunOrder = 622;
                    MainMenu_Change();
                }
            }
            else if (m_iFunOrder == 20)
            {
                if (m_iGrphic_Sel < 14)
                {
                    if (m_iGrphic_Sel % 2 == 0)
                    {
                        m_iGrphic_Sel++;
                        Draw_Grphic_Setup();
                    }
                    else
                    {
                        m_iGrphic_Sel++;
                        Draw_Grphic_Setup();
                    }
                }

                //选择EXIT
                else if (m_iGrphic_Sel == 14)
                {
                    //所有参数恢复默认值
                    m_fDispRNG = 1.5f;
                    m_iBRG_Disp = 0;
                    m_iSORT_Disp = 0;
                    m_fGuardRNG = 12;
                    m_iNumOfShip = 22;
                    m_iContrast = 7;
                    m_bAutoRNG = false;
                    m_iFunOrder = 2;
                    Draw_My_Screen(m_Struct_No1);//绘制界面
                    rectangle_ChoseSign_Top.Visibility = Visibility.Visible;
                    Data_Display();
                    m_iGrphic_Sel = 0;
                    RealTime_Disp();
                }
                //选择ENT
                else if (m_iGrphic_Sel == 16)
                {
                    m_iFunOrder = 2;
                    Draw_My_Screen(m_Struct_No1);//绘制界面
                    rectangle_ChoseSign_Top.Visibility = Visibility.Visible;
                    Data_Display();
                    m_iGrphic_Sel = 0;
                    RealTime_Disp();
                    //修改Title显示
                    if (m_iBRG_Disp == 0)
                    {
                        if (m_iSORT_Disp == 0)
                        {
                            m_iTitle_Choose = 1;
                        }
                        else if (m_iSORT_Disp == 1)
                        {
                            m_iTitle_Choose = 3;
                        }
                        else
                        {
                            m_iTitle_Choose = 5;
                        }
                    }
                    else
                    {
                        if (m_iSORT_Disp == 0)
                        {
                            m_iTitle_Choose = 2;
                        }
                        else if (m_iSORT_Disp == 1)
                        {
                            m_iTitle_Choose = 4;
                        }
                        else
                        {
                            m_iTitle_Choose = 6;
                        }
                    }
                }
            }
            else if (m_iFunOrder == 3)
            {
                m_Detail_Choose = 0;
                TitleDisplay(m_iTitle_Choose);
                m_iFunOrder = 1;
                Draw_My_Screen(m_Struct_No1);//绘制界面

            }
            else if (m_iFunOrder == 4)
            {
                if (m_iSelect == 0)
                {
                    m_iFunOrder = 1;
                    Draw_My_Screen(m_Struct_No1);//绘制界面
                    Draw_Choose(0);
                    Data_Display();
                }
                else if (m_iSelect == 1)
                {
                    m_iFunOrder = 41;
                    m_iList_Select = 0;
                    grid_4_Down_List.Visibility = Visibility.Visible;

                    warpPanel_4_Down.Visibility = Visibility.Collapsed;
                    grid_4_Down.Visibility = Visibility.Collapsed;
                    Draw_Select();



                }
                else if (m_iSelect == 2)
                {
                    m_iFunOrder = 420;
                    warpPanel_4_Down_Owndisp.Visibility = Visibility.Visible;
                    grid_4_Down_Ownship.Visibility = Visibility.Visible;
                    warpPanel_4_Down.Visibility = Visibility.Collapsed;
                    grid_4_Down.Visibility = Visibility.Collapsed;

                    OwnposDisp_Select();


                }
                else if (m_iSelect == 3)
                {

                    m_iFunOrder = 43;

                    TitleDisplay(8);
                    Draw_My_Screen(m_Struct_No1);//绘制界面               
                    Data_Display();

                }
                else if (m_iSelect == 4 || m_iSelect == 5)
                {
                    Data_Display();

                }



            }
            else if (m_iFunOrder == 420)
            {
                if (m_iOwnpos == 1)
                {
                    m_bOwnposdisp = false;
                    m_iFunOrder = 4;
                    m_Count_Choose_Down = 15;
                    m_Count_Choose_Up = 0;
                    m_iSelect = 0;
                    Draw_Select();

                    polygon_ChoseSign_Down_Copy.Visibility = Visibility.Collapsed;
                    Draw_My_Screen(m_Struct_No1);//绘制界面               
                    Data_Display();

                }
                else if (m_iOwnpos == 0)
                {
                    m_bOwnposdisp = true;
                    m_iFunOrder = 4;
                    m_iSelect = 0;

                    m_Count_Choose_Down = 13;
                    m_Count_Choose_Up = 0;
                    Draw_Select();
                    polygon_ChoseSign_Down_Copy.Visibility = Visibility.Visible;

                    Draw_My_Screen(m_Struct_No1);//绘制界面               
                    Data_Display();
                }





            }

            else if (m_iFunOrder == 43)//数据翻页，本船详细信息界面
            {
                m_iFunOrder = 4;
                m_iSelect = 0;
                m_Detail_Choose = 0;
                Draw_My_Screen(m_Struct_No1);
                Data_Display();
                Draw_Select();
                TitleDisplay(m_iTitle_Choose);

            }
            else if (m_iFunOrder == 41)//List菜单界面
            {
                if (m_iList_Select == 0)
                {
                    m_iList_Select = 3;
                    m_iFunOrder = 411;
                    Draw_Select();
                }
                else if (m_iList_Select == 1)
                {
                    m_iList_Select = 5;
                    m_iFunOrder = 412;
                    Draw_Select();
                }
                else if (m_iList_Select == 2)
                {
                    m_iList_Select = 8;
                    m_iFunOrder = 413;
                    Draw_Select();
                }

            }
            else if (m_iFunOrder == 411)//List菜单界面
            {
                if (m_iList_Select == 3)
                {
                    m_iBRG_Disp = 1;
                    if (m_iSORT_Disp == 0)
                    {
                        m_iTitle_Choose = 2;
                        TitleDisplay(m_iTitle_Choose);

                    }
                    else if (m_iSORT_Disp == 1)
                    {
                        m_iTitle_Choose = 4;
                        TitleDisplay(m_iTitle_Choose);
                    }
                    else if (m_iSORT_Disp == 2)
                    {
                        m_iTitle_Choose = 6;
                        TitleDisplay(m_iTitle_Choose);
                    }
                }
                else if (m_iList_Select == 4)
                {
                    m_iBRG_Disp = 0;
                    if (m_iSORT_Disp == 0)
                    {
                        m_iTitle_Choose = 1;
                        TitleDisplay(m_iTitle_Choose);

                    }
                    else if (m_iSORT_Disp == 1)
                    {
                        m_iTitle_Choose = 3;
                        TitleDisplay(m_iTitle_Choose);
                    }
                    else if (m_iSORT_Disp == 2)
                    {
                        m_iTitle_Choose = 5;
                        TitleDisplay(m_iTitle_Choose);
                    }
                }
            }
            else if (m_iFunOrder == 412)//List菜单界面
            {
                if (m_iList_Select == 5)
                {
                    m_iSORT_Disp = 0;
                    if (m_iBRG_Disp == 0)
                    {
                        m_iTitle_Choose = 1;
                        TitleDisplay(m_iTitle_Choose);

                    }
                    else if (m_iBRG_Disp == 1)
                    {
                        m_iTitle_Choose = 2;
                        TitleDisplay(m_iTitle_Choose);
                    }
                }
                else if (m_iList_Select == 6)
                {
                    m_iSORT_Disp = 1;
                    if (m_iBRG_Disp == 0)
                    {
                        m_iTitle_Choose = 3;
                        TitleDisplay(m_iTitle_Choose);

                    }
                    else if (m_iBRG_Disp == 1)
                    {
                        m_iTitle_Choose = 4;
                        TitleDisplay(m_iTitle_Choose);
                    }
                }
                else if (m_iList_Select == 7)
                {
                    m_iSORT_Disp = 2;
                    if (m_iBRG_Disp == 0)
                    {
                        m_iTitle_Choose = 5;
                        TitleDisplay(m_iTitle_Choose);

                    }
                    else if (m_iBRG_Disp == 1)
                    {
                        m_iTitle_Choose = 6;
                        TitleDisplay(m_iTitle_Choose);
                    }
                }


            }

            else if (m_iFunOrder == 413)//List菜单界面
            {
                if (m_iList_Select == 8)
                {
                    m_iNAME_Disp = 0;
                    Data_Display();

                }
                if (m_iList_Select == 9)
                {
                    m_iNAME_Disp = 1;
                    Data_Display();
                }
            }
            else if (m_iFunOrder == 5)//输入密码关机
            {
                if (m_iPassword_sel < 4)
                {
                    m_iPassword_sel++;
                    Draw_Password();

                }
                if (m_strPassCodeinPut.Length < 4)//已输入密码记录
                {
                    m_strPassCodeinPut += (warpPanel_5_Input.Children[m_iKeyBoard_Sel] as TextBlock).Text;
                }
            }
            else if (m_iFunOrder == 7)
            {
                if (m_iKeyBoard_Sel < 66)
                {
                    textBlock_InputText.Text += (warpPanel_Input.Children[m_iKeyBoard_Sel] as TextBlock).Text;
                }
                else if (m_iKeyBoard_Sel == 66)
                {
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                    Grid_Input.Visibility = Visibility.Collapsed;
                    Grid_6.Visibility = Visibility.Visible;
                }
                else if (m_iKeyBoard_Sel == 67)
                {
                    m_strOwnshipData[11] = textBlock_InputText.Text;
                    m_iFunOrder = 60;
                    MainMenu_Change();
                    Draw_MainMenu();
                    Grid_Input.Visibility = Visibility.Collapsed;
                    Grid_6.Visibility = Visibility.Visible;
                }
                else if (m_iKeyBoard_Sel == 68)
                {
                    textBlock_InputText.Text = "";
                }

            }
            else if (m_iFunOrder == 70)
            {
                if (m_iKeyBoard_Sel < 66)
                {
                    textBlock_InputText.Text += (warpPanel_Input.Children[m_iKeyBoard_Sel] as TextBlock).Text;
                }
                else if (m_iKeyBoard_Sel == 66)
                {
                    m_iFunOrder = 61;
                    MainMenu_Change();
                    Draw_MainMenu();
                    Grid_TX.Visibility = Visibility.Collapsed;
                    Grid_Input.Visibility = Visibility.Collapsed;
                    Grid_6.Visibility = Visibility.Visible;
                }
                else if (m_iKeyBoard_Sel == 67)
                {
                    Save_Message();
                    m_iFunOrder = 61;
                    MainMenu_Change();
                    Draw_MainMenu();
                    Grid_TX.Visibility = Visibility.Collapsed;
                    Grid_Input.Visibility = Visibility.Collapsed;
                    Grid_6.Visibility = Visibility.Visible;
                }

            }
         
            if (m_strPassCode == m_strPassCodeinPut)
            {
           
               m_bPower = true;                           
            }

        }//Enter按键左键按下事件
        
        //绘制旋钮逻辑值
        int Count_Rotate = 0;

        bool m_bIsShowDoubleHand = false;
        //双手操作功能及函数
        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            Rect m_Rect = new Rect(0, 0, Grid_Main.Width, Grid_Main.Height);//相对位置
            Point p = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
            if ((m_Rect.Contains(p)) && (m_bIsShowDoubleHand == false))
            {
                StreamResourceInfo sri = Application.GetResourceStream(new Uri("pack://application:,,,/JRC_JHS182_AIS_D;component/Images/双鼠标.cur", UriKind.Absolute));
                Cursor customCursor = new Cursor(sri.Stream);
                Grid_Main.Cursor = customCursor;
                m_bIsShowDoubleHand = true;
            }
            else if (m_bIsShowDoubleHand == true)
            {
                Grid_Main.Cursor = Cursors.Hand;
                m_bIsShowDoubleHand = false;
                image_HandOFF.Visibility = Visibility.Collapsed;
                image_HandPow.Visibility = Visibility.Collapsed;
                image4_ON.Visibility = Visibility.Collapsed;
                image5_ON.Visibility = Visibility.Collapsed;
                image4.Visibility = Visibility.Collapsed;
                image5.Visibility = Visibility.Collapsed;
            }


            
        }

        private void Btn_Enter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Draw_My_Button(1, 12);
        }

        private void Btn_Rt_left_MouseMove(object sender, MouseEventArgs e)
        {
            left.Background = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/left.png", UriKind.RelativeOrAbsolute)));
        }

        private void Btn_Rt_left_MouseLeave(object sender, MouseEventArgs e)
        {
            left.Background = null; 
        }

        private void Btn_Rt_right_MouseMove(object sender, MouseEventArgs e)
        {
            right.Background = new ImageBrush(new BitmapImage(new Uri(@"../JRC_JHS182_AIS_D/Images/right.png", UriKind.RelativeOrAbsolute)));
        }

        private void Btn_Rt_right_MouseLeave(object sender, MouseEventArgs e)
        {
            right.Background = null;
        }

        private void Btn_Stk_up_MouseMove(object sender, MouseEventArgs e)
        {
            imageup.Visibility = Visibility.Visible;
        }

        private void Btn_Stk_up_MouseLeave(object sender, MouseEventArgs e)
        {
            imageup.Visibility = Visibility.Collapsed;
        }

        private void Btn_Stk_left_MouseMove(object sender, MouseEventArgs e)
        {
            imageleft.Visibility = Visibility.Visible;
        }

        private void Btn_Stk_left_MouseLeave(object sender, MouseEventArgs e)
        {
            imageleft.Visibility = Visibility.Collapsed;
        }

        private void Btn_Stk_down_MouseMove(object sender, MouseEventArgs e)
        {
            imagedown.Visibility = Visibility.Visible;
        }

        private void Btn_Stk_down_MouseLeave(object sender, MouseEventArgs e)
        {
            imagedown.Visibility = Visibility.Collapsed;
        }

        private void Btn_Stk_right_MouseMove(object sender, MouseEventArgs e)
        {
            imageright.Visibility = Visibility.Visible;
        }

        private void Btn_Stk_right_MouseLeave(object sender, MouseEventArgs e)
        {
            imageright.Visibility = Visibility.Collapsed;
        }

  


   


       


     

       

     


    }
}
