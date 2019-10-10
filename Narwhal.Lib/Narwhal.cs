using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Medallion.Shell;

namespace narwhal
{
    public class Narwhal
    {
        private bool _quiet;

        /// <summary>
        /// Create an Narwhal instance
        /// </summary>
        /// <param name="quiet">whether to print progress to screen</param>
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

        /// <summary>
        /// Loads a tar.gz into the volume provided
        /// </summary>
        /// <param name="volumeName">volume of the name to load into</param>
        /// <param name="pathToTar">the path of the tarball to load</param>
        /// <returns></returns>
        public IEnumerable<string> Load(string volumeName, string pathToTar)
        {
            var errors = new List<string>();
            var name = "narwhal-mounting-" + Guid.NewGuid();
            try
            {
                StartContainer(name, volumeName, "alpine");
                try
                {
                    CopyToContainer(name, pathToTar);
                    UnzipVolume(name, pathToTar);
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

        /// <summary>
        /// Saves the volume as a tar zip
        /// </summary>
        /// <param name="volumeName">named volume to save</param>
        /// <param name="tarName">the name of the tar zip</param>
        /// <param name="location">the directory to save to</param>
        /// <returns></returns>
        public IEnumerable<string> Save(string volumeName, string tarName = "data", string location = "./")
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
            if (!kill.Result.Success)
            {
                throw new Exception(kill.Result.StandardError);
            }

            Print("Container killed!");
        }


        private void RemoveContainer(string name)
        {
            Print("Remove Container...");
            var remove = Command.Run("docker", "rm", name);
            remove.Wait();
            if (!remove.Result.Success)
            {
                throw new Exception(remove.Result.StandardError);
            }

            Print("Container Removed...");
        }

        private void StartContainer(string name, string mount, string image)
        {
            Print("Creating container to connect to volume...");
            var startContainer = Command.Run("docker", "run", "-d", "-t", "--name", name, "-v",
                mount + ":/home/data", image);
            startContainer.Wait();
            if (!startContainer.Result.Success)
            {
                throw new Exception(startContainer.Result.StandardError);
            }

            Print("Container Created!");
        }

        private void UnzipVolume(string name, string location)
        {
            Print("Unzipping volume...");
            var file = Path.GetFileName(location);
            var rename = Command.Run("docker", "exec", "-w", "/home", name, "mv", file, "data.tar.gz");
            rename.Wait();
            if (!rename.Result.Success)
            {
                throw new Exception(rename.Result.StandardError);
            }

            var zipVolume = Command.Run("docker", "exec", "-w", "/home", name, "tar", "-xzf", "data.tar.gz");
            zipVolume.Wait();
            if (!zipVolume.Result.Success)
            {
                throw new Exception(zipVolume.Result.StandardError);
            }

            Print("Volume Unzipped!");
        }

        private void CopyToContainer(string name, string location)
        {
            Print("Copying to container");
            var cpToHost = Command.Run("docker", "cp", location, name + ":/home/");
            cpToHost.Wait();
            if (!cpToHost.Result.Success)
            {
                throw new Exception(cpToHost.Result.StandardError);
            }

            Print("Done copying!");
        }

        private void CopyToHost(string name, string zippedName, string location)
        {
            Print("Copying to host");
            var cpToHost = Command.Run("docker", "cp", name + ":/home/" + zippedName, location);
            cpToHost.Wait();
            if (!cpToHost.Result.Success)
            {
                throw new Exception(cpToHost.Result.StandardError);
            }

            Print("Done copying!");
        }

        private void ZipVolume(string name, string zippedName)
        {
            Print("Zipping volume...");
            var zipVolume = Command.Run("docker", "exec", "-w", "/home", name, "tar", "-czf", zippedName,
                "data");
            zipVolume.Wait();
            if (!zipVolume.Result.Success)
            {
                throw new Exception(zipVolume.Result.StandardError);
            }

            Print("Volume Zipped!");
        }
    }
}