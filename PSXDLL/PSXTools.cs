﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PSXDLL
{
    public delegate void DestroyDelegate(Client client);
    public delegate void UpdataUrlLog(UrlInfo urlinfo);

    public class PSXTools
    {


        /// <summary>
        /// Get local IP
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetHostIp()
        {
            IPHostEntry ip = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> iplist = ip.AddressList.Where(p => p.AddressFamily == AddressFamily.InterNetwork).ToList();
            return iplist;
        }

        /// <summary>
        /// matching link
        /// </summary>
        /// <returns></returns>
        public static bool RegexUrl(string? urls)
        {
            string[]? rules = AppConfig.Instance().Rule!.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (rules.Length <= 0 || string.IsNullOrEmpty(urls))
            {
                return false;
            }

            return
                rules.Select(rule => new Regex(rule.ToLower().Replace(".", @"\.").Replace("*", ".*?")))
                     .Any(regex => regex.Match(urls.ToLower()).Success);
        }
    }
}
