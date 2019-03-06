using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MOBA_CSharp_Server.Library.DataReader
{
    public class YAMLReader
    {
        Dictionary<string, YAMLObject> cache = new Dictionary<string, YAMLObject>();

        public YAMLObject GetYAMLObject(string path)
        {
            if(File.Exists(path))
            {
                if(cache.ContainsKey(path))
                {
                    return cache[path];
                }
                else
                {
                    var input = new StreamReader(path, Encoding.UTF8);
                    var yaml = new YamlStream();
                    yaml.Load(input);
                    YamlMappingNode rootNode = (YamlMappingNode)yaml.Documents[0].RootNode;

                    YAMLObject yamlObject = new YAMLObject();

                    {
                        var nodes = rootNode.Children[new YamlScalarNode("bool")];
                        var map = nodes as YamlMappingNode;
                        if(map != null)
                        {
                            foreach (var node in map)
                            {
                                yamlObject.AddData(node.Key.ToString(), bool.Parse(node.Value.ToString()));
                            }
                        }
                    }

                    {
                        var nodes = rootNode.Children[new YamlScalarNode("float")];
                        var map = nodes as YamlMappingNode;
                        if (map != null)
                        {
                            foreach (var node in map)
                            {
                                yamlObject.AddData(node.Key.ToString(), float.Parse(node.Value.ToString()));
                            }
                        }
                    }

                    {
                        var nodes = rootNode.Children[new YamlScalarNode("int")];
                        var map = nodes as YamlMappingNode;
                        if (map != null)
                        {
                            foreach (var node in map)
                            {
                                yamlObject.AddData(node.Key.ToString(), int.Parse(node.Value.ToString()));
                            }
                        }
                    }

                    {
                        var nodes = rootNode.Children[new YamlScalarNode("string")];
                        var map = nodes as YamlMappingNode;
                        if (map != null)
                        {
                            foreach (var node in map)
                            {
                                yamlObject.AddData(node.Key.ToString(), node.Value.ToString());
                            }
                        }
                    }

                    input.Close();

                    return yamlObject;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
