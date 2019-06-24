using System;
using System.Data;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.CQS;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using System.Numerics;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Ipfs.Http;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace My_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PopulateTreeView();
            this.treeView1.NodeMouseClick +=
    new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
        }
        #region File Explorer
        private void PopulateTreeView()
        {
            TreeNode rootNode;
            DirectoryInfo info = new DirectoryInfo(@"D:\OneDrive\Деятельность\Учеба\НИР\Проектная папка\My_Project\Documents");
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs,
            TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        void treeView1_NodeMouseClick(object sender,
    TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"),
             new ListViewItem.ListViewSubItem(item,
                dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"),
             new ListViewItem.ListViewSubItem(item,
                file.LastAccessTime.ToShortDateString())};

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        #endregion

        private void BtnCreateDoc_Click(object sender, EventArgs e) // Создать док
        {
            CreateDocument createDocument = new CreateDocument(this);
            createDocument.Show();            
        }

        private void ListView1_ItemActivate(object sender, EventArgs e) // Смотреть док
        {
            ListViewItem selected = listView1.SelectedItems[0];
            string selectedFilePath = @"D:\OneDrive\Деятельность\Учеба\НИР\Проектная папка\My_Project\Documents\" + selected.Text;
            ViewDoc viewDoc = new ViewDoc(this,selectedFilePath);
            viewDoc.Show();
        }

        #region IPFS acsses
        string file_path;
        string file_hash;
        private void BtnSenDoc_Click(object sender, EventArgs e)
        {
            ListViewItem selected = listView1.SelectedItems[0];
            file_path = @"D:\OneDrive\Деятельность\Учеба\НИР\Проектная папка\My_Project\Documents\" + selected.Text;

            Thread myThread = new Thread(AccessingIPFS); //Создаем новый объект потока (Thread)
            myThread.Start();
            myThread.Join();
            label1.Text = file_hash;
        }
        public void AccessingIPFS()
        {
            IpfsClient ipfs = new IpfsClient("https://ipfs.infura.io:5001/api/");
            file_hash = ipfs.FileSystem.AddFileAsync(file_path).Result.Id.Hash.ToString();
        }
        #endregion

        #region Nethereum 
        private async void BtnSenHash_Click(object sender, EventArgs e) // Отправка в блокчейн
        {
            var senderAddress = "0xB07B3D397ce9dF942F18Ae4C14046Fd9Be8e3AE8";            
            var abi = @"[{""constant"":false,""inputs"":[{""name"":""x"",""type"":""string""}],""name"": ""setHash"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"": true,""inputs"":[],""name"":""getHash"",""outputs"":[{""name"":""x"",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""}]";
            var contractAddress = "0x88E719CD4BbA19e53ce6a577835fE8B310773552";

            var url = "https://ropsten.infura.io/v3/09da4421bf974886ab083fdbd016cf79";
            var privateKey = "DC8D4C19C0DF74F26C63EDE7D2689967E0B75FEA6DCC39A304C450A0AE6456DE";
            var account = new Account(privateKey);
            var web3 = new Web3(account, url);

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var setHash = contract.GetFunction("setHash");
            var transactionHash = await setHash.SendTransactionAsync(senderAddress, new HexBigInteger(100000), new HexBigInteger(0), file_hash);

        }
        #endregion
    }
}
