using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PADI_DSTM;

namespace ClientForms
{
    public partial class Form1 : Form
    {
        Dictionary<int, PadInt> padInts = new Dictionary<int, PadInt>();

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            PadiDstm.masterPort = masterPort.Text;
            PadiDstm.masterHostname = masterHost.Text;
            PadiDstm.Init();
            appendToLog("Connected to Master.");
        }

        private void appendToLog(string text)
        {
            log.Text += text + "\r\n";
        }

        private void buttonFail_Click(object sender, EventArgs e)
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
            if(PadiDstm.TxBegin()) {
                appendToLog("Transaction started.");
            }
            else
            {
                appendToLog("Failed to start transaction.");
            }
        }

        private void buttonTxCommit_Click(object sender, EventArgs e)
        {
            if (PadiDstm.TxCommit())
            {
                appendToLog("Transaction committed.");
            }
            else
            {
                appendToLog("Failed to commit transaction.");
            }
        }

        private void buttonTxAbort_Click(object sender, EventArgs e)
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
            int uid = Convert.ToInt32(padIntUId.Text);
            PadInt padInt = PadiDstm.CreatePadInt(uid);
            if(padInt == null)
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
