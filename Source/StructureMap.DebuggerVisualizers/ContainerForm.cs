﻿using System.Windows.Forms;
using System.Linq;

namespace StructureMap.DebuggerVisualizers
{
    public partial class ContainerForm : Form
    {

        public ContainerForm()
        {
            InitializeComponent();
        }

        public ContainerForm(ContainerDetail containerDetail)
        {
            InitializeComponent();
            buildTree(BrowserTree, containerDetail);
        }

        private static void buildTree(TreeView tree, ContainerDetail container)
        {
            var sourcesRoot = buildConfigurationSources(tree, container);
            var pluginTypesRoot = buildPluginTypes(tree, container);

            pluginTypesRoot.Expand();
            sourcesRoot.Expand();
        }

        private static TreeNode buildPluginTypes(TreeView tree, ContainerDetail container)
        {
            var pluginTypesRoot = tree.Nodes.Add("PluginTypes");
            foreach (var pluginType in container.PluginTypes.OrderBy(t => t.Type.Name))
            {
                addPluginType(pluginTypesRoot, pluginType);
            }
            return pluginTypesRoot;
        }

        private static TreeNode buildConfigurationSources(TreeView tree, ContainerDetail container)
        {
            var sourcesRoot = tree.Nodes.Add("Configuration Sources");
            foreach (var source in container.Sources.OrderBy(s => s))
            {
                addSource(sourcesRoot, source);
            }
            return sourcesRoot;
        }

        private static void addSource(TreeNode root, string source)
        {
            root.Nodes.Add(source);
        }

        private static void addPluginType(TreeNode pluginTypesRoot, PluginTypeDetail pluginType)
        {
            var pluginNode = pluginTypesRoot.Nodes.Add(pluginType.Type.AsCSharp());
            pluginNode.Nodes.Add("FullName: " + pluginType.Type.AsCSharp(t=>t.FullName ?? t.Name));
            pluginNode.Nodes.Add("Assembly: " + pluginType.Type.Assembly);
            pluginNode.Nodes.Add("BuildPolicy: " + pluginType.BuildPolicy.Name);
            if (pluginType.Instances.Length == 0) return;
            
            var instancesRoot = pluginNode.Nodes.Add("Instances");
            foreach (var instance in pluginType.Instances.OrderBy(i => i.Name))
            {
                addInstance(instancesRoot, instance);
            }
        }

        private static void addInstance(TreeNode instancesRoot, InstanceDetail instance)
        {
            var instanceNode = instancesRoot.Nodes.Add("Name: " + instance.Name);
            if (instance.Name != instance.Description)
            {
                instanceNode.Nodes.Add("Description: " + instance.Description);
            }
            if (instance.ConcreteType != null)
            {
                instanceNode.Nodes.Add("ConcreteType: " + instance.ConcreteType.AsCSharp());
            }
        }

        private void BrowserTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level < 1) return;
            e.Node.ExpandAll();
        }
    }
}
