using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    public class Log
    {
        Dictionary<long, Dictionary<int, int>> oldValues = new Dictionary<long, Dictionary<int, int>>();
        Dictionary<long, Dictionary<int, long>> oldWriteTimestamps = new Dictionary<long, Dictionary<int, long>>();

        public void AddLogEntry(long timestamp)
        {
            oldValues.Add(timestamp, new Dictionary<int, int>());
            oldWriteTimestamps.Add(timestamp, new Dictionary<int, long>());
            Console.WriteLine("Add log entry to timestamp: " + timestamp);
        }

        public void RemoveLogEntry(long timestamp)
        {
            oldValues.Remove(timestamp);
            oldWriteTimestamps.Remove(timestamp);
            Console.WriteLine("Removed log entry from timestamp: " + timestamp);
        }

        public void StorePadInt(long timestamp, int uid, int val, long writeTimestamp)
        {
            if (!oldValues[timestamp].Keys.Contains(uid))
            {
                oldValues[timestamp].Add(uid, val);
                oldWriteTimestamps[timestamp].Add(uid, writeTimestamp);
                Console.WriteLine("Stored padint: " + uid + " with val: " + val + "and writetimestamp: " + writeTimestamp + " to timestamp: " + timestamp);
            }
        }

        public void RestorePadInt(PadInt p, long timestamp, int uid)
        {
            if (p.writeTimestamp > oldWriteTimestamps[timestamp][uid])
            {
                p.value = oldValues[timestamp][uid];
                p.writeTimestamp = oldWriteTimestamps[timestamp][uid];
                Console.WriteLine("Restored padint: " + uid + " to val: " + p.value + "and writetimestamp: " + p.writeTimestamp + " in timestamp: " + timestamp);
            }
        }

        public Dictionary<int, int> GetTimestampUIDs(long timestamp)
        {
            return oldValues[timestamp];
        }

        public long GetOldTimestamp(int uid)
        {
            foreach (long timestamp in oldWriteTimestamps.Keys)
            {
                /* Protects against uninitialized entries */
                if (oldWriteTimestamps[timestamp].Count > 0)
                {
                    Console.WriteLine("Got old timstamp of padint: " + uid + " in timestamp: " + timestamp);
                    return oldWriteTimestamps[timestamp][uid];
                }
            }
            return -1;
        }

        public int GetOldValue(int uid)
        {
            foreach (long timestamp in oldValues.Keys)
            {
                /* Protects against uninitialized entries */
                if (oldValues[timestamp].Count > 0)
                {
                    Console.WriteLine("Got old value of padint: " + uid + " in timestamp: " + timestamp);
                    return oldValues[timestamp][uid];
                }
            }
            return -1;
        }

        public bool Contains(long timestamp)
        {
            return oldValues.Keys.Contains(timestamp);
        }

        public bool ContainsPadInt(int uid)
        {
            foreach (long timestamp in oldWriteTimestamps.Keys)
            {
                if (oldWriteTimestamps[timestamp].Keys.Contains(uid))
                {
                    Console.WriteLine("Found padint: " + uid + " in timestamp: " + timestamp);
                    return true;
                }
            }
            return false;
        }
    }
}
