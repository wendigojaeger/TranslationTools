using System.Windows.Controls;

namespace WendigoJaeger.TranslationTool.Extensions
{
    static class ItemContainerGeneratorExtension
    {
        public static TreeViewItem ContainerFromItemRecursive(this ItemContainerGenerator root, object item)
        {
            var treeViewItem = root.ContainerFromItem(item) as TreeViewItem;
            if (treeViewItem != null)
            {
                return treeViewItem;
            }

            foreach(var child in root.Items)
            {
                treeViewItem = root.ContainerFromItem(child) as TreeViewItem;

                var search = treeViewItem?.ItemContainerGenerator.ContainerFromItemRecursive(item);
                if (search != null)
                {
                    return search;
                }
            }

            return null;
        }
    }
}
