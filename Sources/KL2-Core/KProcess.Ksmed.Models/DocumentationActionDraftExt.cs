using MoreLinq;
using System.Linq;

namespace KProcess.Ksmed.Models
{
    public partial class DocumentationActionDraft
    {
        public string DurationString { get; set; }

        public void Delete()
        {
            ChangeTracker.ChangeTrackingEnabled = true;
            ReferentialDocumentations.ToList().ForEach(dbDocRef =>
            {
                dbDocRef.ChangeTracker.ChangeTrackingEnabled = true;
                dbDocRef.MarkAsDeleted();
            });
            this.MarkAsDeleted();
        }

        public DocumentationActionDraft(DocumentationActionDraft reference)
        {
            Label = reference.Label;
            Duration = reference.Duration;
            ThumbnailHash = reference.ThumbnailHash;
            IsKeyTask = reference.IsKeyTask;
            SkillId = reference.SkillId;
            CustomNumericValue = reference.CustomNumericValue;
            CustomNumericValue2 = reference.CustomNumericValue2;
            CustomNumericValue3 = reference.CustomNumericValue3;
            CustomNumericValue4 = reference.CustomNumericValue4;
            CustomTextValue = reference.CustomTextValue;
            CustomTextValue2 = reference.CustomTextValue2;
            CustomTextValue3 = reference.CustomTextValue3;
            CustomTextValue4 = reference.CustomTextValue4;
            ResourceId = reference.ResourceId;
            ActionCategoryId = reference.ActionCategoryId;

            reference.ReferentialDocumentations.ForEach(docRef => ReferentialDocumentations.Add(new ReferentialDocumentationActionDraft(docRef)));
        }

        public void Update(DocumentationActionDraft reference)
        {
            ChangeTracker.ChangeTrackingEnabled = true;

            Label = reference.Label;
            Duration = reference.Duration;
            if (Thumbnail?.Hash != reference.Thumbnail?.Hash)
                Thumbnail = reference.Thumbnail;
            IsKeyTask = reference.IsKeyTask;
            SkillId = reference.SkillId;
            CustomNumericValue = reference.CustomNumericValue;
            CustomNumericValue2 = reference.CustomNumericValue2;
            CustomNumericValue3 = reference.CustomNumericValue3;
            CustomNumericValue4 = reference.CustomNumericValue4;
            CustomTextValue = reference.CustomTextValue;
            CustomTextValue2 = reference.CustomTextValue2;
            CustomTextValue3 = reference.CustomTextValue3;
            CustomTextValue4 = reference.CustomTextValue4;
            ResourceId = reference.ResourceId;
            ActionCategoryId = reference.ActionCategoryId;

            ReferentialDocumentations.ToList().ForEach(docRef =>
            {
                if (!reference.ReferentialDocumentations.Any(_ => _.RefNumber == docRef.RefNumber && _.ReferentialId == docRef.ReferentialId))
                {
                    docRef.ChangeTracker.ChangeTrackingEnabled = true;
                    docRef.MarkAsDeleted();
                }
            });
            reference.ReferentialDocumentations.ForEach(docRef =>
            {
                var oldDocRef = ReferentialDocumentations.SingleOrDefault(_ => _.RefNumber == docRef.RefNumber && _.ReferentialId == docRef.ReferentialId);
                if (oldDocRef == null)
                    ReferentialDocumentations.Add(new ReferentialDocumentationActionDraft
                    {
                        RefNumber = docRef.RefNumber,
                        ReferentialId = docRef.ReferentialId,
                        Quantity = docRef.Quantity
                    });
                else
                {
                    oldDocRef.ChangeTracker.ChangeTrackingEnabled = true;
                    oldDocRef.Quantity = docRef.Quantity;
                }
            });
        }
    }
}
