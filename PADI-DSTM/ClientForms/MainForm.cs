using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PADI_DSTM;

namespace ClientForms
{
    public delegate void AsyncDelegate();

    public partial class MainForm : Form
    {
        Dictionary<int, PadInt> padInts = new Dictionary<int, PadInt>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Connect);
            thread.Start();
        }

        private void Connect()
        {
            AsyncDelegate connect = new AsyncDelegate(ConnectAsync);
            BeginInvoke(connect);
        }

        private void ConnectAsync()
        {
            PadiDstm.masterPort = masterPort.Text;
            PadiDstm.masterHostname = masterHost.Text;
            PadiDstm.Init();
            appendToLog("Connected to Master.");
        }

        private void appendToLog(string text)
        {
            log.AppendText(text + "\r\n");
        }

        private void buttonFail_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Fail);
            thread.Start();
        }

        private void Fail()
        {
            AsyncDelegate fail = new AsyncDelegate(FailAsync);
            BeginInvoke(fail);
        }

        private void FailAsync()
        {
            string url = "tcp://" + serverHost.Text + ":" + serverPort.Text + "/RemoteDataServer";
            if (PadiDstm.Fail(url))
            {
                appendToLog("Server shutdown at " + url + ".");
            }
            else
            {
                appendToLog("Failed to shutdown server at " + url + ".");
            }
        }

        private void buttoFreeze_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Freeze);
            thread.Start();
        }

        private void Freeze()
        {
            AsyncDelegate freezeDelegate = new AsyncDelegate(FreezeAsync);
            BeginInvoke(freezeDelegate);
        }

        public void FreezeAsync()
        {
            string url = "tcp://" + serverHost.Text + ":" + serverPort.Text + "/RemoteDataServer";
            if (PadiDstm.Freeze(url))
            {
                appendToLog("Server freeze at " + url + ".");
            }
            else
            {
                appendToLog("Failed to freeze server at " + url + ".");
            }
        }

        private void buttonRecover_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Recover);
            thread.Start();
        }

        private void Recover()
        {
            AsyncDelegate recover = new AsyncDelegate(RecoverAsync);
            BeginInvoke(recover);
        }

        private void RecoverAsync()
        {
            string url = "tcp://" + serverHost.Text + ":" + serverPort.Text + "/RemoteDataServer";
            if (PadiDstm.Recover(url))
            {
                appendToLog("Server recover at " + url + ".");
            }
            else
            {
                appendToLog("Failed to recover server at " + url + ".");
            }
        }

        private void buttonTxbegin_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Begin);
            thread.Start();
        }

        private void Begin()
        {
            AsyncDelegate begin = new AsyncDelegate(BeginAsync);
            BeginInvoke(begin);
        }

        private void BeginAsync()
        {
            if (PadiDstm.TxBegin())
            {
                appendToLog("Transaction started.");
            }
            else
            {
                appendToLog("Failed to start transaction.");
            }
        }

        private void buttonTxCommit_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Commit);
            thread.Start();
        }

        private void Commit()
        {
            AsyncDelegate commit = new AsyncDelegate(CommitAsync);
            BeginInvoke(commit);
        }

        private void CommitAsync()
        {
            if (PadiDstm.TxCommit())
            {
                appendToLog("Transaction committed.");
            }
            else
            {
                appendToLog("Failed to commit transaction: Transaction Aborted.");
            }
        }

        private void buttonTxAbort_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Abort);
            thread.Start();
            
        }

        private void Abort()
        {
            AsyncDelegate abort = new AsyncDelegate(AbortAsync);
            BeginInvoke(abort);
        }


        private void AbortAsync()
        {
            if (PadiDstm.TxAbort())
            {
                appendToLog("Transaction aborted.");
            }
            else
            {
                appendToLog("Failed to abort transaction.");
            }

        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Create);
            thread.Start();
        }

        private void Create()
        {
            AsyncDelegate create = new AsyncDelegate(CreateAsync);
            BeginInvoke(create);
        }

        private void CreateAsync()
        {
            int uid = Convert.ToInt32(padIntUId.Text);
            PadInt padInt = PadiDstm.CreatePadInt(uid);
            if (padInt == null)
            {
                appendToLog("Failed to create PadInt with uid: " + uid + ".");
            }
            else
            {
                padInts.Add(uid, padInt);
                appendToLog("Created PadInt with uid: " + uid + ".");
            }
        }

        private void buttonAccess_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Access);
            thread.Start();
        }

        private void Access()
        {
            AsyncDelegate access = new AsyncDelegate(AccessAsync);
            BeginInvoke(access);
        }

        private void AccessAsync()
        {
            int uid = Convert.ToInt32(padIntUId.Text);
            PadInt padInt = PadiDstm.AccessPadInt(uid);
            if (padInt == null)
            {
                appendToLog("Failed to access PadInt with uid: " + uid + ".");
            }
            else
            {
                if (!padInts.ContainsKey(uid))
                {
                    padInts.Add(uid, padInt);
                }
                appendToLog("Accessed PadInt with uid: " + uid + ".");
            }
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Read);
            thread.Start();
            
        }

        private void Read()
        {
            AsyncDelegate read = new AsyncDelegate(ReadAsync);
            BeginInvoke(read);
        }

        private void ReadAsync()
        {
            int uid = Convert.ToInt32(padIntUId.Text);
            try
            {
                int value = padInts[uid].Read();
                appendToLog("Read value: " + value + " at PadInt with uid: " + uid + ".");
            }
            catch (Exception ex)
            {
                appendToLog(ex.Message);
            }
        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(this.Write);
            thread.Start();
        }

        private void Write()
        {
            AsyncDelegate write = new AsyncDelegate(WriteAsync);
            BeginInvoke(write);
        }

        private void WriteAsync()
        {
            int uid = Convert.ToInt32(padIntUId.Text);
            int value = Convert.ToInt32(padIntValue.Text);
            try
            {
                padInts[uid].Write(value);
                appendToLog("Wrote value: " + value + " at PadInt with uid: " + uid + ".");
            }
            catch (Exception ex)
            {
                appendToLog(ex.Message);
            }
        }
    }
}
