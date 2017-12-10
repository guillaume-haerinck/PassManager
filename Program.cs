using System;

namespace ConsoleApp.SQLite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ModifyDb crud = new ModifyDb();

            //DEBUG Need to put the console writeline into the functions
            if (args.Length >= 2)
            {
                switch (args[0])
                {
                    case "-r":
                        //System.Console.WriteLine("Processing registration of user");
                        if (crud.IsUserValid(args, true))
                        {
                            //User already exist can not create twice
                            System.Console.WriteLine("ERROR");
                        }
                        else
                        {
                            //User never created
                            crud.AddUserAndMasterPassword(args);
                        }
                        break;

                    case "-a":
                        //System.Console.WriteLine("Processing add to database");
                        if (crud.IsUserValid(args, true))
                        {
                            //DEBUG not implemented yet
                            /*
                            if (crud.IsTagValid(args))
                            {
                                //Tag already exist, cannot create twice
                                System.Console.WriteLine("ERROR");
                            }
                            else
                            {
                            */
                                crud.AddTagAndPassword(args);
                        }
                        else
                        {
                            System.Console.WriteLine("ERROR user");
                        }
                        break;

                    case "-g":
                        //System.Console.WriteLine("Processing get data");
                        if ((crud.IsUserValid(args, true)) && (crud.IsTagValid(args)))
                        {
                            //DEBUG must only get the tags of the current user
                            crud.GetDecryptedPassword(args);
                        }
                        else
                        {
                            System.Console.WriteLine("ERROR");
                        }
                        break;

                    case "-d":
                        //System.Console.WriteLine("Processing delete data");
                        if ((crud.IsUserValid(args, true)) && (crud.IsTagValid(args)))
                        {
                            crud.DeleteTagAndPassword(args);
                        }
                        else
                        {
                            System.Console.WriteLine("ERROR");
                        }
                        break;

                    case "-t":
                        //System.Console.WriteLine("Processing get cryptography");
                        //DEBUG problem in user checking
                        /*if (crud.IsUserValid(args, false))
                        {*/
                            if (args.Length == 3)
                            {
                                //Get the salt and the hash of the MasterPassword of the user
                                crud.GetSaltAndHash(args);
                            }
                            else if (args.Length == 4)
                            {
                                //Get the encrypted password corresponding to the tag
                                crud.GetEncryptedPassword(args);
                            }
                            else
                            {
                                System.Console.WriteLine("ERROR");
                            }
                        break;

                    default:
                        //System.Console.WriteLine("Flag not recognized");
                        System.Console.WriteLine("ERROR");
                        break;
                }
            }
            //if there is not enough argument to process any query
            else
            {
                System.Console.WriteLine("ERROR");
            }
        }
    }
}