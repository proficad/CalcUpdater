using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace CalcUpdater
{
    class Program
    {
        static string m_path_updater_file_path;

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new Exception("expecting 2 params: input and output path!");
            }

            string ls_currFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //!!! remove this line before use in production!!!
            //ls_currFolder = @"W:\P\i18n\";

            string ls_pathIn = Path.Combine(ls_currFolder, args[0].ToString());

            

            if (!File.Exists(ls_pathIn))
            {
                string ls_msg = "file does not exist: " + ls_pathIn;
                throw new Exception(ls_msg);
            }

//            string ls_pathOut = args[1].ToString();

            string ls_pathOut = Path.Combine(ls_currFolder, args[1].ToString());

            CalcFile(ls_pathIn, ls_pathOut);
        }

        private static void CalcFile(string ls_pathIn, string ls_pathOut)
        {
            m_path_updater_file_path = Path.GetDirectoryName(ls_pathIn);
            string ls_contents = File.ReadAllText(ls_pathIn);
            MatchEvaluator l_eval = new MatchEvaluator(MakeOneReplace);
            ls_contents = Regex.Replace(ls_contents, "hash=\"(.*)\"", l_eval);

            File.WriteAllText(ls_pathOut, ls_contents, System.Text.Encoding.Unicode);
            Console.WriteLine("Calculated " + ls_pathOut);
        }

        private static string MakeOneReplace(Match as_match)
        {
            string ls_path = as_match.Groups[1].ToString();
            string ls_fullPath = Path.Combine(m_path_updater_file_path, ls_path);

            if (!File.Exists(ls_fullPath))
            {
                string ls_msg = "file does not exist: " + ls_fullPath;
                throw new Exception(ls_msg);
            }

            string ls_hash = CalcFileMd5(ls_fullPath);

            return string.Format("hash=\"{0}\"", ls_hash);
        }

        private static string CalcFileMd5(string ls_fullPath)
        {
            string ls_md5Result;
            StringBuilder sb = new StringBuilder();
            MD5 md5Hasher = MD5.Create();

            using (FileStream fs = File.OpenRead(ls_fullPath))
            {
                foreach (Byte b in md5Hasher.ComputeHash(fs))
                    sb.Append(b.ToString("x2").ToLower());
            }

            ls_md5Result = sb.ToString();

            return ls_md5Result;
        }

    }



}
