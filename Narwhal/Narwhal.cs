using System;
using System.Collections.Generic;
using System.Diagnostics;
using Medallion.Shell;

namespace narwhal
{
    public class Narwhal
    {
        private bool _quiet;

        public Narwhal(bool quiet)
        {
            _quiet = quiet;
        }

        private void Print(string c)
        {
            if (!_quiet)
            {
                Console.WriteLine(c);
            }
        }

        public IEnumerable<string> Save(string volumeName, string tarName, string location)
        {
            var errors = new List<string>();
            var name = "narwhal-mounting-" + Guid.NewGuid();
            var zippedName = $"{tarName}.tar.gz";

            try
            {
                StartContainer(name, volumeName, "alpine");
                try
                {
                    ZipVolume(name, zippedName);
                    CopyToHost(name, zippedName, location);
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);
                }
                finally
                {
                    KillContainer(name);
                    RemoveContainer(name);
                }
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
            }

            return errors;
        }

        private void KillContainer(string name)
        {
            Print("Killing container...");
            var kill = Command.Run("docker", "kill", name);
            kill.Wait();
            Print("Container killed!");
        }


        private void RemoveContainer(string name)
        {
            Print("Remove Container...");
            var remove = Command.Run("docker", "rm", name);
            remove.Wait();
            Print("Container Removed...");
        }

        private void StartContainer(string name, string mount, string image)
        {
            Print("Creating container to connect to volume...");
            var startContainer = Command.Run("docker", "run", "-d", "-t", "--name", name, "-v",
                mount + ":/home/data", image);
            startContainer.Wait();
            Print("Container Created!");
        }

        private void CopyToHost(string name, string zippedName, string location)
        {
            Print("Copying to host");
            var cpToHost = Command.Run("docker", "cp", name + ":/home/" + zippedName, location);
            cpToHost.Wait();
            Print("Done copying!");
        }

        private void ZipVolume(string name, string zippedName)
        {
            Print("Zipping volume...");
            var zipVolume = Command.Run("docker", "exec", "-w", "/home", name, "tar", "-czf", zippedName,
                "data");
            zipVolume.Wait();
            Print("Volume Zipped!");
        }
    }
}