// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;

class CtorTest
{
    public static int Main(string[] args)
    {
        // Check args
        if (args.Length < 2)
        {
            Console.WriteLine("USAGE: SemaphoreCtor4 /iCount:<int> /mCount:<int> " + 
                "[/semName:<string>] [/iRandom:<int>]");
            return -1;
        }

        // Get the args
        int iCount = -1, mCount = -1, iRandom = -1;
        string semName = "DefaultString";

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].ToLower().StartsWith("/icount:"))
            {
                iCount = Convert.ToInt32(args[i].Substring(8));
                continue;
            }

            if (args[i].ToLower().StartsWith("/mcount:"))
            {
                mCount = Convert.ToInt32(args[i].Substring(8));
                continue;
            }

            if (args[i].ToLower().StartsWith("/irandom:"))
            {
                iRandom = Convert.ToInt32(args[i].Substring(9));
                continue;
            }

            if (args[i].ToLower().StartsWith("/semname:"))
            {
                semName = args[i].Substring(9);
                if (semName.ToLower() == "null")
                    semName = null;
                continue;
            }

            if (args[i].ToLower().StartsWith("/unisemname:"))
            {
                semName = string.Empty;
                //    Convert to unicode
                string[] s = args[i].Substring(12).Split(';');
                foreach (string str in s)
                    semName += Convert.ToChar(Convert.ToInt32(str));
                continue;
            }
        }
        CtorTest ct = new CtorTest();
        return ct.Run(iCount, mCount, semName, iRandom);
    }
           
    private int Run(int iCount, int mCount, string semName, int iRandom)
    {
        // Testing createdNew
        bool createdNew;
        int iRet = -1, count = iCount;
        Semaphore sem = null;
        if (iRandom > 0)
            semName = Common.GenerateUnicodeString(iRandom);
        try
        {
            using(sem = new Semaphore(iCount, mCount, semName, out createdNew))
            {
                if (iCount > 0)
                {
                    sem.WaitOne();
                    count--;
                }

                int iPrev = sem.Release();
                if (iPrev == count && createdNew)
                    iRet = 100;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected exception thrown:  " + ex.ToString());
        }

        Console.WriteLine(100 == iRet ? "Test Passed" : "Test Failed");
        return iRet;
    }
}
