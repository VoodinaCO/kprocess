using Kprocess.KL2.TabletClient.Models;
using KProcess.Ksmed.Models;
using System.Windows;
using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class PublishedRefItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element)
            {
                if (item is PublishedReferentialAction refAction)
                {
                    if (refAction.PublishedReferential.FileHash != null && ExtensionsUtil.IsImageExtension(refAction.PublishedReferential.File.Extension))
                        return element.FindResource("ImageTemplate") as DataTemplate;
                    if (refAction.PublishedReferential.FileHash != null)
                        return element.FindResource("UriTemplate") as DataTemplate;
                    return element.FindResource("TextTemplate") as DataTemplate;
                }
                else if (item is PublishedResource pResource)
                {
                    if (pResource.FileHash != null && ExtensionsUtil.IsImageExtension(pResource.File.Extension))
                        return element.FindResource("ResourceImageTemplate") as DataTemplate;
                    if (pResource.FileHash != null)
                        return element.FindResource("ResourceUriTemplate") as DataTemplate;
                    return element.FindResource("ResourceTextTemplate") as DataTemplate;
                }
                else if (item is PublishedActionCategory pCategory)
                {
                    if (pCategory.FileHash != null && ExtensionsUtil.IsImageExtension(pCategory.File.Extension))
                        return element.FindResource("CategoryImageTemplate") as DataTemplate;
                    if (pCategory.FileHash != null)
                        return element.FindResource("CategoryUriTemplate") as DataTemplate;
                    return element.FindResource("CategoryTextTemplate") as DataTemplate;
                }
                else if (item is Skill skill)
                {
                    if (skill.Hash != null && ExtensionsUtil.IsImageExtension(skill.CloudFile.Extension))
                        return element.FindResource("SkillImageTemplate") as DataTemplate;
                    if (skill.Hash != null)
                        return element.FindResource("SkillUriTemplate") as DataTemplate;
                    return element.FindResource("SkillTextTemplate") as DataTemplate;
                }
            }
            return null;
        }
    }
}
