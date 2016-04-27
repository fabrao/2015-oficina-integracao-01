using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Kinect;
using Microsoft.Kinect.Interop;
using IrrKlang;


using System.ComponentModel;
using System.Data;

using System.Linq;
using System.Text;

using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Media;
using System.Speech.Synthesis;



namespace KinectTest
{

	public partial class MainForm : Form
    {
		KinectSensor MySensor;
        int timeleft = 3;
        Thread t = null;
        Skeleton skeleton;




        public MainForm()
        {
			InitializeComponent();

			MySensor = KinectSensor.KinectSensors[0];
            if (MySensor != null)
            {
                MySensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                MySensor.AllFramesReady += FramesReady;

                MySensor.SkeletonStream.Enable();

                MySensor.Start();
            }
		}


		void FramesReady(object sender, AllFramesReadyEventArgs e)
        {

            using (ColorImageFrame VFrame = e.OpenColorImageFrame())
            {
                if (VFrame == null)
                    return;
                byte[] pixelS = new byte[VFrame.PixelDataLength];
                Bitmap bmap = ImageToBitmap(VFrame);

                if (!((KinectSensor)sender).SkeletonStream.IsEnabled)
                {
                    return;
                }

                using (SkeletonFrame SFrame = e.OpenSkeletonFrame())
                {

                    if (SFrame == null) return;


                    Graphics g = Graphics.FromImage(bmap);

                    Skeleton[] Skeletons = new Skeleton[SFrame.SkeletonArrayLength];

                    SFrame.CopySkeletonDataTo(Skeletons);
                    Color cor = System.Drawing.Color.Aquamarine;

                    foreach (Skeleton S in Skeletons)
                    {

                        if (S.TrackingState == SkeletonTrackingState.Tracked)
                        {

                            //Desenha Corpo
                            DrawBone(JointType.Head, JointType.ShoulderCenter, S, g);
                            DrawBone(JointType.ShoulderCenter, JointType.Spine, S, g);
                            DrawBone(JointType.Spine, JointType.HipCenter, S, g);

                            //Desenha Braço Esquerdo
                            DrawBone(JointType.ShoulderCenter, JointType.ShoulderLeft, S, g);
                            DrawBone(JointType.ShoulderLeft, JointType.ElbowLeft, S, g);
                            DrawBone(JointType.ElbowLeft, JointType.WristLeft, S, g);
                            DrawBone(JointType.WristLeft, JointType.HandLeft, S, g);

                            //Desenha Braço Direito
                            DrawBone(JointType.ShoulderCenter, JointType.ShoulderRight, S, g);
                            DrawBone(JointType.ShoulderRight, JointType.ElbowRight, S, g);
                            DrawBone(JointType.ElbowRight, JointType.WristRight, S, g);
                            DrawBone(JointType.WristRight, JointType.HandRight, S, g);

                            //Desenha Perna Esquerda
                            DrawBone(JointType.HipCenter, JointType.HipRight, S, g);
                            DrawBone(JointType.HipRight, JointType.KneeRight, S, g);
                            DrawBone(JointType.KneeRight, JointType.AnkleRight, S, g);
                            DrawBone(JointType.AnkleRight, JointType.FootRight, S, g);

                            //Desenha Perna Direita
                            DrawBone(JointType.HipCenter, JointType.HipLeft, S, g);
                            DrawBone(JointType.HipLeft, JointType.KneeLeft, S, g);
                            DrawBone(JointType.KneeLeft, JointType.AnkleLeft, S, g);
                            DrawBone(JointType.AnkleLeft, JointType.FootLeft, S, g);

                            //Desenha VÉRTICES
                            foreach (Joint j in S.Joints)
                            {
                                DrawJoint(j.JointType, S, g, cor);
                            }

                            skeleton = S;
                            if (t == null){
                                    t = new Thread(new ThreadStart(isFall));
                                    t.Start();
                            }
                        }
                    }
                }
                campo.Image = bmap;
            }

		}


        //FUNÇÃO P/ DETECTAR QUEDA
        void isFall(){
            float mediax = 0, mediaantigax = 0, mediaz = 0, mediaantigaz = 0;
            float mediay = 0, mediaantigay = 0, espinhaantigo = 0, cabeca = 0 ;
            int quantidade = 0;
            while (true){
                mediax = 0; mediaantigax = 0; mediay= 0; mediaantigay = 0; 
                mediaz = 0; mediaantigaz = 0; quantidade = 0;

                #region SALVA MediaX, MediaY e MediaZ DAS VÉRTICES (MÉDIA ANTIGA)
                foreach (Joint j in skeleton.Joints)
                {
                    mediaantigax += j.Position.X;
                    mediaantigay += j.Position.Y;
                    mediaantigaz += j.Position.Z;
                    quantidade++;
                    if (j.JointType == JointType.Spine)
                    {
                        espinhaantigo = j.Position.Y;
                    }
                }
                mediaantigax = mediaantigax / quantidade;
                mediaantigay = mediaantigay / quantidade;
                mediaantigaz = mediaantigaz / quantidade;
                #endregion
                Thread.Sleep(500);
                #region SALVA MediaX, MediaY e MediaZ DAS VÉRTICES (NOVA MÉDIA: APÓS 500ms)
                foreach (Joint j in skeleton.Joints)
                {
                    mediax += j.Position.X;
                    mediay += j.Position.Y;
                    mediaz += j.Position.Z;
                    if (j.JointType == JointType.Head)
                    {
                        cabeca = j.Position.Y;
                    }
                }
                mediax = mediax / quantidade;
                mediay = mediay / quantidade;
                mediaz = mediaz / quantidade;
                #endregion

                //Estabelece as seguintes regras:
								//Se houve movimentação repentina (para baixo) no eixo y E
								//O ponto da cabeça está significativamente mais baixo 
                            //comparado com o ponto da espinha armazenado 0,5s antes E
								// Houve movimentação repentina no eixo x OU no eixo Z
								// Se true, então  ocorreu uma queda
                if (((mediay < (mediaantigay - 0.1)) && (cabeca < (espinhaantigo - 0.2))) && 
                    ((movimentacao(mediax,mediaantigax) || movimentacao(mediaz,mediaantigaz))))
                {
                    System.Diagnostics.Debug.WriteLine("Queda detectada!!!"); //Imprime no prompt
                    avisoQueda(); //Alerta sonoro (speak)
                }
            }
        }
        //FUNÇÃO PARA DETECTAR SE HOUVE UM GRANDE DESLOCAMENTO ENTRE AS MÉDIAS
        Boolean movimentacao(float media, float mediaantiga){
            if (media > mediaantiga + 0.2){
                return true;
            }else if (media < mediaantiga - 0.2){
                return true;
            }
            return false;
        }
        //FUNÇÃO PARA ALERTAR SOBRE A QUEDA
        private void avisoQueda() {
            SpeechSynthesizer voz = new SpeechSynthesizer();
            voz.Volume = 100;
            voz.Rate = -2;
            voz.Speak("Queda detectada!");
        }


        //FUNÇÃO P/ DESENHAR ARESTAS DO ESQUELETO
		void DrawBone(JointType j1, JointType j2, Skeleton S, Graphics g)
        {
            Point p1 = GetJoint(j1, S);
            Point p2 = GetJoint(j2, S);
            Pen myPen = new Pen(System.Drawing.Color.Blue, 5);
            g.DrawLine(myPen, p1, p2);
        }

        //FUNÇÃO P/ DESENHAR VÉRTICES DO ESQUELETO
        void DrawJoint(JointType j, Skeleton s, Graphics g, Color c)
        {
            Point p = GetJoint(j, s);
            Pen myPen = new Pen(c, 5);
            g.DrawEllipse(myPen, p.X, p.Y, 5, 5);
        }

        //FUNÇÃO P/ SELECIONAR LOCALIZAÇÃO CARTESIANA DA VÉRTICE (X,Y)
		Point GetJoint(JointType j, Skeleton S)
        {
            SkeletonPoint Sloc = S.Joints[j].Position;
            ColorImagePoint Cloc = MySensor.MapSkeletonPointToColor(Sloc, ColorImageFormat.RgbResolution640x480Fps30);
            return new Point(Cloc.X, Cloc.Y);
        }



	    private Bitmap ImageToBitmap(ColorImageFrame Image)
        {
            byte[] pixeldata = new byte[Image.PixelDataLength];
            Image.CopyPixelDataTo(pixeldata);
            Bitmap bmap = new Bitmap(Image.Width, Image.Height, PixelFormat.Format32bppRgb);

            BitmapData bmapdata = bmap.LockBits(new Rectangle(0, 0,Image.Width, Image.Height), ImageLockMode.WriteOnly, bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(pixeldata, 0, ptr,Image.PixelDataLength);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }
	}
}
