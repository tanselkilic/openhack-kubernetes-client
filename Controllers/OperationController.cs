using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        //GET api/operation/list
        [HttpGet("list", Name = "list")]
        public ActionResult<string> Get()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(config);
            var list1 = client.ListNamespacedPodAsync("default").Result;
            var list = client.ListNamespace();

            var json = JsonConvert.SerializeObject(list);
            return json;
        }

        //GET api/operation/createnamespace
        [HttpGet("createnamespace", Name = "CreateNamespace")]
        public ActionResult<IEnumerable<string>> CreateNamespace()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(config);
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "test"
                }
            };
            var result = client.CreateNamespace(ns);
            return new string[] { "Operation", "Namespace Created" };
        }

        //GET api/operation/deletenamespace
        [HttpGet("deletenamespace", Name = "deleteNamespace")]
        public ActionResult<IEnumerable<string>> deleteNamespace()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(config);
            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "test"
                }
            };
            var status = client.DeleteNamespace(new V1DeleteOptions(), ns.Metadata.Name);
            return new string[] { "Operation", "Namespace Deleted" };
        }

        //GET api/operation/createpod
        [HttpGet("createpod", Name = "Createpod")]
        public async Task<ActionResult<IEnumerable<string>>> CreatePod()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            var client = new Kubernetes(config);
            var pod = new V1Pod()
            {
                ApiVersion = "v1",
                Kind = "Pod",
                Metadata = new V1ObjectMeta()
                {
                    Name = "minecraft-deployment"
                },
                Spec = new V1PodSpec()
                {
                    Containers = new List<V1Container>()
                    {
                        new V1Container()
                        {
                            Name = "minecraft-deploymentcontiner",
                            Image = "openhack/minecraft-server:1.0",
                            VolumeMounts = new List<V1VolumeMount>()
                            {
                                new V1VolumeMount()
                                {
                                    MountPath = "/data",
                                    Name = "volume"
                                }
                            },
                            Ports = new List<V1ContainerPort>()
                            {
                                new V1ContainerPort()
                                {
                                    HostPort = 25565,
                                    ContainerPort = 25565
                                },
                                new V1ContainerPort()
                                {
                                    HostPort = 25575,
                                    ContainerPort = 25575
                                }
                            },
                            Env = new List<V1EnvVar>()
                            {
                                new V1EnvVar()
                                {
                                    Name = "EULA",
                                    Value = "TRUE"
                                }
                            } 
                        }
                    },
                    Volumes = new List<V1Volume>()
                    {
                        new V1Volume()
                        {
                            Name = "volume",
                            PersistentVolumeClaim= new V1PersistentVolumeClaimVolumeSource()
                            {
                                ClaimName = "azure-managed-disk"
                            }
                        }
                    },
                    RestartPolicy = "Never"
                }
            };

            try
            {
                var result = await client.CreateNamespacedPodAsync(pod, "default");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return new string[] { "Operation", "Pod Created" };
        }

    }
}
