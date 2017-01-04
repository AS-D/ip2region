using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ip2region
{
    public class ip2region
    {
        const int INDEX_BLOCK_LENGTH = 12;
        const int TOTAL_HEADER_LENGTH = 4096;
        string dbFile;
        byte[] dbBinStr;
        long firstIndexPtr, lastIndexPtr, totalBlocks;
        public ip2region(string path)
        {
            dbFile = path;
        }

        public IpInfo MemorySearch(string ipStr)
        {
            IpInfo ipInfo = null;
            long dataPtr = 0;
            if (totalBlocks == 0)
            {
                dbBinStr = File.ReadAllBytes(dbFile);
                firstIndexPtr = getLong(dbBinStr, 0);
                lastIndexPtr = getLong(dbBinStr, 4);
                totalBlocks = (lastIndexPtr - firstIndexPtr) / INDEX_BLOCK_LENGTH + 1;
            }
            var ip = ip2long(ipStr);

            var h = totalBlocks;
            long l = 0;

            while (l <= h)
            {
                var m = (l + h) >> 1;
                var p = firstIndexPtr + m * INDEX_BLOCK_LENGTH;
                var sip = getLong(dbBinStr, p);
                if (ip < sip)
                {
                    h = m - 1;
                }
                else
                {
                    var eip = getLong(dbBinStr, p + 4);
                    if (ip > eip)
                    {
                        l = m + 1;
                    }
                    else
                    {
                        dataPtr = getLong(dbBinStr, p + 8);
                        break;
                    }
                }
            }

            if (dataPtr != 0)
            {
                var dataLen = ((dataPtr >> 24) & 0xff);
                dataPtr = (dataPtr & 0x00ffffff);
                var len = dataLen - 4;
                byte[] x=new byte[len]; 
                Array.Copy(dbBinStr, dataPtr + 4, x, 0, len);
                ipInfo = getIpInfo(getLong(dbBinStr, dataPtr), x);
            }
            return ipInfo;
        }

        public IpInfo getIpInfo(long cityId, byte[] line)
        {
            var lineSlice = System.Text.Encoding.UTF8.GetString(line).Split('|');
            var ipInfo = new IpInfo();
            ipInfo.CityId = cityId;
            if (lineSlice.Count() < 5)
            {
                var a = new string[5];
                lineSlice.CopyTo(a, 0);
                lineSlice = a;
            }

            ipInfo.Country = lineSlice[0];
            ipInfo.Region = lineSlice[1];
            ipInfo.Province = lineSlice[2];
            ipInfo.City = lineSlice[3];
            ipInfo.ISP = lineSlice[4];
            return ipInfo;
        }
        public long ip2long(string IpStr)
        {
            var bits = IpStr.Split('.');
            if (bits.Count() != 4)
            {
                return 0;
            }
            long sum = 0;
            for (int i = 0; i < bits.Count(); i++)
            {
                var bit = Int64.Parse(bits[i]);
                sum += bit << (24 - 8 * i);
            }
            return sum;
        }

        public long getLong(byte[] b, long offset)
        {
            return (long.Parse(b[offset].ToString()) |
                long.Parse(b[offset + 1].ToString()) << 8 |
                long.Parse(b[offset + 2].ToString()) << 16 |
                long.Parse(b[offset + 3].ToString()) << 24);
        }
    }

    public class IpInfo
    {
        public long CityId { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string ISP { get; set; }
    }
}
