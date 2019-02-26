using System;
using Xunit;
using bluzelle_cs;

namespace bluzelle_cs.Tests
{
    public class CrudTests
    {
        [Fact]
        public void Test1()
        {
            var testentry = "ws://localhost:50000";
            var testpemkey = "MHQCAQEEIFyA1k5rGxesnmaWzRM9AJLyu9IznuU7VMDpFWBXL+6UoAcGBSuBBAAKoUQDQgAEvkjTgUwQHcezTd9iGLEMOwZjG6/sb2veyB8kSR13fVuqHJYPpeUC73js+US3QKp1CFIbvVEwj0Zmtp8+YjS62A==";
            var testuuid = "testuuid";

            Console.WriteLine("");
            Console.WriteLine("******C# Bluzelle Test******");

            var swarm = new Bluzelle(testentry,testpemkey,testuuid);

            do
            {
                Console.WriteLine("");
                Console.WriteLine("0 - Exit");
                Console.WriteLine("1 - Swarm.CreateDB");
                Console.WriteLine("2 - Swarm.DeleteDB");
                Console.WriteLine("3 - Swarm.HasDB");
                Console.WriteLine("------------------");
                Console.WriteLine("4 - Swarm.Writers");
                Console.WriteLine("5 - Swarm.AddWriters");
                Console.WriteLine("6 - Swarm.RemoveWriters");
                Console.WriteLine("------------------");
                Console.WriteLine("7 - Swarm.Create");
                Console.WriteLine("8 - Swarm.Read");
                Console.WriteLine("9 - Swarm.QuickRead");
                Console.WriteLine("10 - Swarm.Update");
                Console.WriteLine("11 - Swarm.Delete");
                Console.WriteLine("------------------");
                Console.WriteLine("12 - Swarm.Size");
                Console.WriteLine("13 - Swarm.Has");
                Console.WriteLine("14 - Swarm.Keys");
                // Console.WriteLine("15 - Swarm.Subscribe");
                // Console.WriteLine("16 - Swarm.Unsubscribe");


                int option;
                
                if (int.TryParse(Console.ReadLine(), out option))
                {
                    switch (option){
                        case 0:
                            {
                                Environment.Exit(0);
                                break;
                            }
                        case 1:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**CREATE DB**");

                                Console.WriteLine("UUID: " + testuuid);

                                var result = swarm.CreateDB().GetAwaiter().GetResult();
                                if (result)
                                {
                                    Console.WriteLine("Updated!");
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Failed!");
                                    Console.WriteLine("");
                                }

                                break;
                            }
                        case 2:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**DELETE DB**");

                                Console.WriteLine("UUID: " + testuuid);

                                var result = swarm.DeleteDB().GetAwaiter().GetResult();
                                if (result)
                                {
                                    Console.WriteLine("Updated!");
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Failed!");
                                    Console.WriteLine("");
                                }

                                break;
                            }
                        case 3:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**Has DB**");

                                Console.WriteLine("UUID: " + testuuid);
                                

                                var result = swarm.HasDB().GetAwaiter().GetResult();
                                if (result)
                                {
                                    Console.WriteLine("Found!");
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Not Found!");
                                    Console.WriteLine("");
                                }

                                break;
                            }
                        case 4:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**Writers**");

                                Console.WriteLine("UUID: " + testuuid);
                                

                                var result = swarm.Writers().GetAwaiter().GetResult();
                                Console.WriteLine(result);

                                break;
                            }
                        case 5:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**Add Writers**");

                                Console.WriteLine("UUID: " + testuuid);
                                

                                Console.Write("Writer: ");
                                var addwriter = Console.ReadLine();

                                var result = swarm.AddWriters(addwriter).GetAwaiter().GetResult();
                                
                                if (result)
                                {
                                    Console.WriteLine("Updated!");
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Failed!");
                                    Console.WriteLine("");
                                }

                                break;
                            }
                        case 6:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**Remove Writers**");

                                Console.WriteLine("UUID: " + testuuid);
                                

                                Console.Write("Writer: ");
                                var removewriter = Console.ReadLine();

                                var result = swarm.RemoveWriters(removewriter).GetAwaiter().GetResult();
                                
                                if (result)
                                {
                                    Console.WriteLine("Updated!");
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Failed!");
                                    Console.WriteLine("");
                                }

                                break;
                            }
                        case 7:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**CREATE**");

                                Console.WriteLine("UUID: " + testuuid);
                                

                                Console.Write("KEY: ");
                                var key = Console.ReadLine();

                                Console.Write("VALUE: ");
                                var val = Console.ReadLine();

                                var result = swarm.Create(key, val).GetAwaiter().GetResult();
                                if (result)
                                {
                                    Console.WriteLine("Updated!");
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Failed!");
                                    Console.WriteLine("");
                                }

                                break;
                            }
                        case 8:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**READ**");

                                Console.WriteLine("UUID: " + testuuid);
                                

                                Console.Write("KEY: ");
                                var key = Console.ReadLine();

                                var val = swarm.Read(key).GetAwaiter().GetResult();
                                if  (val != null)
                                {
                                    Console.WriteLine("GOT: " + val);
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Nothing found");
                                    Console.WriteLine("");
                                }

                                break;
                            }
                        case 9:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**QUICK READ**");

                                Console.WriteLine("UUID: " + testuuid);
                                

                                Console.Write("KEY: ");
                                var key = Console.ReadLine();

                                var val = swarm.QuickRead(key).GetAwaiter().GetResult();
                                if  (val != null)
                                {
                                    Console.WriteLine("GOT: " + val);
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Nothing found");
                                    Console.WriteLine("");
                                }

                                break;
                            }
                        case 10:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**UPDATE**");

                                Console.WriteLine("UUID: " + testuuid);
                                

                                Console.Write("KEY: ");
                                var key = Console.ReadLine();

                                Console.Write("VALUE: ");
                                var val = Console.ReadLine();

                                var result = swarm.Update(key, val).GetAwaiter().GetResult();
                                if (result)
                                {
                                    Console.WriteLine("Updated!");
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Failed!");
                                    Console.WriteLine("");
                                }

                                break;
                            }

                        case 11:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**DELETE**");

                                Console.WriteLine("UUID: " + testuuid);
                                

                                Console.Write("KEY: ");
                                var key = Console.ReadLine();

                                var result = swarm.Delete(key).GetAwaiter().GetResult();
                                if (result)
                                {
                                    Console.WriteLine("Removed!");
                                    Console.WriteLine("");
                                }
                                else
                                {
                                    Console.WriteLine("Failed!");
                                    Console.WriteLine("");
                                }

                                break;
                            }
                        case 12:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**Size**");

                                Console.WriteLine("UUID: " + testuuid);
                                

                                var result = swarm.Size().GetAwaiter().GetResult();
                                Console.WriteLine(result);

                                break;
                            }
                        case 13:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**Has Key**");

                                Console.WriteLine("UUID: " + testuuid);
                                
                                Console.Write("KEY: ");
                                var key = Console.ReadLine();

                                var result = swarm.Has(key).GetAwaiter().GetResult();
                                Console.WriteLine(result);

                                break;
                            }
                        case 14:
                            {
                                Console.WriteLine("");
                                Console.WriteLine("**Keys List**");

                                Console.WriteLine("UUID: " + testuuid);
                                
                                var result = swarm.Keys().GetAwaiter().GetResult();
                                Console.WriteLine(result);

                                break;
                            }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid option");
                }

            } while (true);
        }
    }
}
