using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    public class LogEntry 
    {
        public long transactionTimestamp;
        public int padIntUID;
        public int padIntValue;

        public LogEntry(long ts, int uid, int val)
        {
            this.transactionTimestamp = ts;
            this.padIntUID = uid;
            this.padIntValue = val;
        }
    }

    public class Log
    {
        List<LogEntry> logEntries = new List<LogEntry>();

        public void AddNewLogEntry(long timestamp, int uid, int value)
        {
            logEntries.Add(new LogEntry(timestamp, uid, value));
        }

        public void RemoveAllEntries(long timestamp)
        {
            List<LogEntry> entriesToRemove = new List<LogEntry>();

            foreach (LogEntry l in logEntries)
            {
                if (l.transactionTimestamp == timestamp)
                {
                    entriesToRemove.Add(l);
                }
            }
            foreach (LogEntry l in entriesToRemove)
            {
                logEntries.Remove(l);
            }
        }

        public int getLastValue(int uid)
        {
            for (int i = logEntries.Count-1; i >= 0; i--)
            {
                if (logEntries[i].padIntUID == uid)
                {
                    return logEntries[i].padIntValue;
                }
            }
            return 0;
        }

        public long getLastWriteTimestamp(int uid)
        {
            for (int i = logEntries.Count - 1; i >= 0; i--)
            {
                if (logEntries[i].padIntUID == uid)
                {
                    return logEntries[i].transactionTimestamp;
                }
            }
            return 0;
        }
    }
}
