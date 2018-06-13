using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proiect_si
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }
        private string readcontent; //continutul fisierului
        private string readpath; //
        private Bitmap image, img,dec_img; //image displays the unmodified picture while img displays the image after the message was hidden
        static int[] binary = new int[8]; //holds the binary rep of the latest elem from byte_array
        static byte[] byte_array; //holds the ASCII rep for the loaded text
        private int w, h; //width & height of the image
        private Color pink; //access the RGB properties of each pixel
        private int contor_array = 0;
        private int contor_binar = 0;
        private int w_char = 0; //counter used to keep track of # char/line
        private int char_line = 0; //number of characters per line
         //coordonate pt parcurgerea imaginii
        /***************************Functii conversie*******************/
        public static byte[] StringToByteArray(string s)
        {
            return CharArrayToByteArray(s.ToCharArray());
        }
        public static byte[] CharArrayToByteArray(char[] array)
        {
            return Encoding.ASCII.GetBytes(array, 0, array.Length);
        }
        public static string ByteArrayToString(byte[] array)
        {
            return Encoding.ASCII.GetString(array);
        }
        public static byte Binary_to_Byte(string s)
        {          
                byte dec = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[s.Length - i - 1] == '0') continue;
                    dec += (byte)Math.Pow(2, i);
                }
                return dec;
        }
        public static int get_LSB(byte x)
        {
            int[] rep = new int[8];
            string s = Convert.ToString(x, 2);
            rep = s.PadLeft(8, '0').Select(c => int.Parse(c.ToString())).ToArray();
            return rep[7];
        }
        public static void Byte_to_binary(byte x)
        {
            string s = Convert.ToString(x, 2);
            binary = s.PadLeft(8, '0').Select(c => int.Parse(c.ToString())).ToArray();
        }
        /*******************EVENT HANDLERS**************/
        private void load_text_Click(object sender, EventArgs e)
        {
            readcontent = String.Empty;
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Text|*.txt";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                readpath = opf.FileName;
                StreamReader sr = new StreamReader(readpath, false);
                readcontent += sr.ReadToEnd();
                sr.Close();
            }
            byte_array = StringToByteArray(readcontent);
            array_length = byte_array.Length;
        }

        private void save_Click(object sender, EventArgs e)
        {
            if(text_decriptat.Length > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Text Document|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(sfd.FileName, true);
                    sw.WriteLine(text_decriptat);
                    sw.Close();
                }
            }
        }
        private int pixel_contor = 0;

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        int array_length;
        private void encrypt_Click(object sender, EventArgs e)
        {
            int width = -1, height = 0;
            int r_LSB, g_LSB, b_LSB;
           // bool b_minus_red = false, b_plus_red = false, b_minus_green = false, b_plus_green = false, b_minus_blue = false, b_plus_blue = false;
            byte red,green,blue;
            //byte minus_red, plus_red, minus_green, plus_green, minus_blue, plus_blue;
            char_line = img.Width / 3; //number of characters/line
            while(contor_array < array_length) //cat timp mai sunt caractere
            {
                Byte_to_binary(byte_array[contor_array]); //conversie in binar (int array global)
                contor_array++; 
                pixel_contor = 0;
                contor_binar = 0;
                if(w_char == char_line) //daca nr de char/linie depaseste limita admisa
                {
                    //trecere pe linie noua si reinit width
                    w_char = 0;
                    height++; //new line
                    width = -1;
                }
                else
                {
                    w_char++; // 1 x w_char = 3 pixels
                }
                while(pixel_contor <= 2) //0,1,2
                { //width to be incremented at the end
                    if (width == img.Width) width = 0;
                    if(pixel_contor == 0 || pixel_contor == 1)
                    {
                        width++;
                                               
                        pink = img.GetPixel(width, height);
                        red = pink.R;
                        green = pink.G;
                        blue = pink.B;
                        r_LSB = get_LSB(red);
                        g_LSB = get_LSB(green);
                        b_LSB = get_LSB(blue);
                        if (pixel_contor == 1) { contor_binar = 3; }                       
                        //contor_binar e pe 0
                        if ((r_LSB == 1 && binary[contor_binar] == 1) || (r_LSB == 0 && binary[contor_binar] == 0))
                        {

                        }
                        if (r_LSB == 1 && binary[contor_binar] == 0)
                        {
                           img.SetPixel(width, height, Color.FromArgb(255, --red, pink.G, pink.B));
                        }
                        if (r_LSB == 0 && binary[contor_binar] == 1)
                        {
                            img.SetPixel(width, height, Color.FromArgb(255, ++red, pink.G, pink.B));
                        }
                        /*************************************************************************/
                        contor_binar++; //1 //4
                        if ((g_LSB == 1 && binary[contor_binar] == 1) || (g_LSB == 0 && binary[contor_binar] == 0))
                        {
                        }
                        if (g_LSB == 1 && binary[contor_binar] == 0)
                        {
                            img.SetPixel(width, height, Color.FromArgb(255, red, --green, pink.B));
                        }
                        if (g_LSB == 0 && binary[contor_binar] == 1)
                        {
                            img.SetPixel(width, height, Color.FromArgb(255, red, ++green, pink.B));
                        }
                        /*************************************************************************/
                        contor_binar++; //2 //5
                        if ((b_LSB == 1 && binary[contor_binar] == 1) || (b_LSB == 0 && binary[contor_binar] == 0))
                        {
                        }
               
                        if (b_LSB == 1 && binary[contor_binar] == 0)
                        {
                            img.SetPixel(width, height, Color.FromArgb(255, red, green, --blue));
                        }
                        if (b_LSB == 0 && binary[contor_binar] == 1)
                        {
                            img.SetPixel(width, height, Color.FromArgb(255, red, green, ++blue));
                        }
                        }
                    if (pixel_contor == 2)
                    {
                        width++;                       
                        pink = img.GetPixel(width, height);
                        red = pink.R;
                        green = pink.G;
                        r_LSB = get_LSB(red);
                        g_LSB = get_LSB(green);
                        contor_binar = 6;
                        if ((r_LSB == 1 && binary[contor_binar] == 1) || (r_LSB == 0 && binary[contor_binar] == 0))
                        {

                        }
                        if (r_LSB == 1 && binary[contor_binar] == 0)
                        {
                            img.SetPixel(width, height, Color.FromArgb(255, --red, green, pink.B));

                        }
                        if (r_LSB == 0 && binary[contor_binar] == 1)
                        {
                            img.SetPixel(width, height, Color.FromArgb(255, ++red, green, pink.B));

                        }
                        /*************************************************************************/
                        contor_binar = 7; //7 END
                        if ((g_LSB == 1 && binary[contor_binar] == 1) || (g_LSB == 0 && binary[contor_binar] == 0))
                        {

                        }
                        if (g_LSB == 1 && binary[contor_binar] == 0)
                        {
                            img.SetPixel(width, height, Color.FromArgb(255, red, --green, pink.B));

                        }
                        if (g_LSB == 0 && binary[contor_binar] == 1)
                        {
                            img.SetPixel(width, height, Color.FromArgb(255, red, ++green, pink.B));

                        }
                    }
                    pixel_contor++;
                }                    
                }
            SaveFileDialog sfd = new SaveFileDialog();
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                img.Save(sfd.FileName + "_" + array_length.ToString() + ".bmp");
            }
            }
        /*while(contor_array < byte_array.Length)
        {
            width = 0;
            contor_binar = 0;
            Byte_to_binary(byte_array[contor_array]);
            while(width < 8)
            {
                pink = img.GetPixel(width, height);
                red = pink.R;
                LSB = get_LSB(red);
                if((LSB == 1 && binary[contor_binar] == 1) || (LSB == 0 && binary[contor_binar] == 0))
                {
                    img.SetPixel(width, height, Color.FromArgb(255,pink.R, pink.G, pink.B));
                }
                if(LSB == 0 && binary[contor_binar] == 1)
                {
                    img.SetPixel(width, height, Color.FromArgb(255, ++red, pink.G, pink.B));
                }
                if(LSB == 1 && binary[contor_binar] == 0)
                {
                    img.SetPixel(width, height, Color.FromArgb(255, --red, pink.G, pink.B));
                }
                width++;
                contor_binar++;
            }
            contor_array++;
            height++;
        }
        SaveFileDialog sfd = new SaveFileDialog();
        if(sfd.ShowDialog() == DialogResult.OK)
        img.Save(sfd.FileName + "_"+ height.ToString() + ".bmp");
        textBox1.Clear();
        MessageBox.Show("SUCCESS");*/
        string text_decriptat = "";
        int contor_decript = 0;
        byte[] decrypt_array;
        private void decrypt_Click(object sender, EventArgs e)
        {
            int nr_char_codate;
            int nr_linii;
            int contor_char = 0;
            int contor_linie = 0;
            int contor_width = 0;
            int d_pixel_contor = 0;
            int nr_char_linie;
            int d_width = 0;
            int d_height = 0;
            int w, h;
            string[] s;
            string[] s1;
            string sec_bin = "";
            Color mycolor;
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "(*.bmp)|*.bmp";
            if(opf.ShowDialog() == DialogResult.OK)
            {
                dec_img = (Bitmap)Image.FromFile(opf.FileName);
                w = dec_img.Width;
                h = dec_img.Height;
                s = opf.FileName.ToString().Split('_');
                s1 = s[1].Split('.');
                nr_char_codate = int.Parse(s1[0]);
                nr_char_linie = img.Width / 3;
                nr_linii = nr_char_codate / nr_char_linie;
                decrypt_array = new byte[nr_char_codate+1];
                while (contor_char < nr_char_codate)
                {
                    d_pixel_contor = 0;
                    contor_char++;
                    if(contor_width >= nr_char_linie)
                    {
                        contor_width = 0;
                        d_width = 0;
                        d_height++;
                    }
                    else
                    {
                        contor_width++;
                    }
                    while(d_pixel_contor <= 2)
                    {
                        if(d_pixel_contor == 0 || d_pixel_contor == 1)
                        {
                            mycolor = dec_img.GetPixel(d_width, d_height);
                            d_width++;
                            sec_bin += get_LSB(mycolor.R);
                            sec_bin += get_LSB(mycolor.G);
                            sec_bin += get_LSB(mycolor.B);
                            d_pixel_contor++;
                        }
                        if(d_pixel_contor == 2)
                        {
                            mycolor = dec_img.GetPixel(d_width, d_height);
                            d_width++;
                            sec_bin += get_LSB(mycolor.R);
                            sec_bin += get_LSB(mycolor.G);
                            decrypt_array[contor_decript] = Binary_to_Byte(sec_bin);
                            contor_decript++;
                            sec_bin = "";
                            d_pixel_contor++;
                        }
                    }                  
                }
            }
            MessageBox.Show("SUCCESS!");
            textBox1.Clear();
            textBox1.Text = ByteArrayToString(decrypt_array);
            /*int img_h;
            int m;
            int he = 0;
            int w = 0;
            string[] s, s1;

            string sec_binara = "";
            OpenFileDialog opf = new OpenFileDialog();
            if (opf.ShowDialog() == DialogResult.OK)
            {
                dec_img = (Bitmap)Image.FromFile(opf.FileName);
                pictureBox1.Image = dec_img;
                s = opf.FileName.ToString().Split('_');
                s1 = s[1].Split('.');
                img_h = int.Parse(s1[0]);
                byte[] decript_array = new byte[img_h + 1];
                while (he < img_h)
                {
                    w = 0;
                    while (w < 8)
                    {
                        m = get_LSB(dec_img.GetPixel(w, he).R);
                        sec_binara += m.ToString();
                        w++;
                    }
                    decript_array[he] = Binary_to_Byte(sec_binara);
                    sec_binara = "";
                    he++;
                }
                text_decriptat = ByteArrayToString(decript_array);
                textBox1.Text = text_decriptat;

            }*/
        }
        private void load_image_Click(object sender, EventArgs e)
        {
            int words;
            string s_words;
            OpenFileDialog opf = new OpenFileDialog();
            opf.FilterIndex = 1;
            if (opf.ShowDialog() == DialogResult.OK)
            {
                image = (Bitmap)Image.FromFile(opf.FileName);
                img = (Bitmap)Image.FromFile(opf.FileName);
                pictureBox1.Image = image;
                w = img.Width;
                h = img.Height;
                words = h * (w / 3);
                s_words = words.ToString();
                MessageBox.Show("# of chars:" + s_words);
                //MessageBox.Show("Width: " + w.ToString());
               // MessageBox.Show("Height: " + h.ToString());
            }
        }
    }

}
