using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace My_Project
{
    public partial class ViewDoc : Form
    {
        public ViewDoc()
        {
            InitializeComponent();
        }
        public ViewDoc(Form1 f1)
        {
            InitializeComponent();
        }
        public ViewDoc(Form1 f1, string path)
        {
            InitializeComponent();

            if (File.Exists(path)) // если существует данный файл
            {
                
                XDocument xDoc = XDocument.Load(path);
                DataSet ds = new DataSet(); // создаем новый пустой кэш данных
                ds.ReadXml(path); // записываем в него XML-данные из файла
                textBox1.Text = xDoc.Element("documents").Attribute("name").Value; // имя документа
                textBox6.Text = xDoc.Element("documents").Attribute("id").Value; // ид документа
                textBox2.Text = xDoc.Element("documents").Element("metaData").Element("registrator").Value;
                textBox3.Text = xDoc.Element("documents").Element("metaData").Element("registr_number").Value;
                textBox4.Text = xDoc.Element("documents").Element("metaData").Element("date_registr").Value;

                try
                {
                    foreach (XElement x in xDoc.Element("documents").Element("fileList").Elements("file"))
                    {
                        linkLabel1.Text = x.Attribute("name").Value;
                        fileLink = "https://ipfs.infura.io/ipfs/" + x.Value;
                    }
                }
                catch
                {
                    linkLabel1.Text = string.Empty;
                }
                try
                {
                    foreach (XElement x in xDoc.Element("documents").Element("references").Elements("ref"))
                    {
                        textBox5.Text += x.Attribute("type").Value + "  " + x.Value + Environment.NewLine;
                    }
                }
                catch
                {
                    linkLabel1.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("XML файл не найден.", "Ошибка.");
            }
        }
        private void ViewDoc_Load(object sender, EventArgs e)
        {

        }
        string fileLink = string.Empty;
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }
        private void VisitLink()
        {
            // Change the color of the link text by setting LinkVisited   
            // to true.  
            linkLabel1.LinkVisited = true;
            //Call the Process.Start method to open the default browser   
            //with a URL:  
            System.Diagnostics.Process.Start(fileLink);
        }
    }
}
