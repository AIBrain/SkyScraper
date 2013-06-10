﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SkyScraper
{
    public static class Robots
    {
        const string DisallowRegex = @"^Disallow:\s";
        const string AllowRegex = @"^Allow:\s";
        static Dictionary<string, string> allRules;

        public static void Load(string robotsTxt, string userAgent = "*")
        {
            allRules = new Dictionary<string, string>();
            var allRulesList = new List<string>();
            var botRulesList = new List<string>();
            string currentAgentName = null;
            robotsTxt = Regex.Replace(robotsTxt, @"\r\n|\n\r|\n|\r", "\r\n");
            var stringReader = new StringReader(robotsTxt);
            while (stringReader.Peek() > -1)
            {
                var line = stringReader.ReadLine() ?? string.Empty;
                var agentNameMatch = Regex.Match(line, @"(?<=^User-agent:\s)[^\s]*");
                if (agentNameMatch.Success)
                {
                    currentAgentName = agentNameMatch.Value;
                    continue;
                }
                if ((currentAgentName == "*" || currentAgentName == userAgent) && line.IsRule())
                    if (currentAgentName == "*")
                        allRulesList.Add(line);
                    else
                        botRulesList.Add(line);
            }
            foreach (var rule in botRulesList.Concat(allRulesList))
            {
                var value = Regex.IsMatch(rule, DisallowRegex) ? rule.AsRegexRule(DisallowRegex) : rule.AsRegexRule(AllowRegex);
                allRules.Add(rule, value);
            }
        }

        public static bool PathIsAllowed(string path)
        {
            foreach (var rule in allRules)
            {
                if (path.IsDisallowed(rule))
                    return false;
                if (path.IsAllowed(rule))
                    return true;
            }
            return true;
        }

        static bool CheckRule(this string path, KeyValuePair<string, string> rule, string regex)
        {
            return Regex.IsMatch(rule.Key, regex) && Regex.IsMatch(path, rule.Value);
        }

        static bool IsDisallowed(this string path, KeyValuePair<string, string> rule)
        {
            return CheckRule(path, rule, DisallowRegex);
        }

        static bool IsAllowed(this string path, KeyValuePair<string, string> rule)
        {
            return CheckRule(path, rule, AllowRegex);
        }

        static bool IsRule(this string input)
        {
            var rules = string.Format("{0}|{1}", DisallowRegex, AllowRegex);
            return Regex.IsMatch(input, rules);
        }

        static string AsRegexRule(this string input, string regex)
        {
            regex = string.Format(@"(?<={0})[^\s]*", regex);
            input = Regex.Match(input, regex).Value;
            input = input.Replace("*", ".*");
            return string.Format("^{0}.*$", input);
        }
    }
}