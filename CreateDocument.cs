using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ipfs.Http;
using System.Threading;
using System.Xml.Linq;

namespace My_Project
{
    public partial class CreateDocument : Form
    {
        public CreateDocument()
        {
            InitializeComponent();
        }
        public CreateDocument(Form1 f1)
        {
            InitializeComponent();
        }
        private void BtnCreateDoc_Click(object sender, EventArgs e) //сохранение данных из формы в XML
        {
            try
            {
                XDocument document = new XDocument(
                new XElement("documents",
                new XAttribute("id", Convert.ToString((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 100000000)),
                new XAttribute("name", textBox1.Text)));

                XElement metaData = new XElement("metaData",
                new XElement("registrator", textBox2.Text),
                new XElement("registr_number", textBox3.Text),
                new XElement("date_registr", dateTimePicker1.Value));
                document.Element("documents").Add(metaData);

                if (filePath != string.Empty)
                {
                    XElement fileList = new XElement("fileList",
                new XElement("file", fileHash,
                new XAttribute("name", fileName)));
                    document.Element("documents").Add(fileList);
                }
                else
                {
                    document.Element("documents").Add(new XElement("fileList"));
                }

                if (dataGridView1.Rows.Count != 0)
                {
                    XElement references = new XElement("references");
                    for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
                    {
                        references.Add(new XElement("ref", dataGridView1["hashOfRef", i].Value,
                new XAttribute("type", dataGridView1["TypeOfRef", i].Value)));                      
                    }
                    document.Element("documents").Add(references);
                }
                                
                string s = Convert.ToString((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
                string s2 = s.Substring(s.Length - 4);

                string docFilePath = @"D:\OneDrive\Деятельность\Учеба\НИР\Проектная папка\My_Project\Documents\doc" + s2 + ".xml";
                document.Save(docFilePath);                 
                MessageBox.Show("XML файл успешно сохранен.", "Выполнено.");
                this.Close();
            }
            catch
            {
                MessageBox.Show("Невозможно сохранить XML файл.", "Ошибка.");
            }
        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        string filePath = string.Empty;
        string fileName = string.Empty;
        string fileHash = string.Empty;
        private void Button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;            

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"d:\\\Downloads";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    fileName = openFileDialog.SafeFileName;
                    filePath = openFileDialog.FileName;                    
                    label6.Text = filePath;
                    Thread myThread = new Thread(AccessingIPFS); //Создаем новый объект потока (Thread)
                    myThread.Start();
                    myThread.Join();
                }
            }
            //MessageBox.Show(fileContent, "File Content at path: " + filePath, MessageBoxButtons.OK);
        }
        public void AccessingIPFS()
        {
            IpfsClient ipfs = new IpfsClient("https://ipfs.infura.io:5001/api/");
            fileHash = ipfs.FileSystem.AddFileAsync(filePath).Result.Id.Hash.ToString();
        }
    }
}
