using System;
using EFT.InventoryLogic;
using EFT.Quests;
using EFTReflection;

// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace EFTApi.Helpers
{
    public class MongoIDHelper
    {
        private static readonly Lazy<MongoIDHelper> Lazy = new Lazy<MongoIDHelper>(() => new MongoIDHelper());

        public static MongoIDHelper Instance => Lazy.Value;

        public ItemMongoIDData ItemMongoIDHelper => ItemMongoIDData.Instance;

        public ConditionMongoIDData ConditionMongoIDHelper => ConditionMongoIDData.Instance;

        private MongoIDHelper()
        {
        }

        public class ItemMongoIDData
        {
            private static readonly Lazy<ItemMongoIDData> Lazy = new Lazy<ItemMongoIDData>(() => new ItemMongoIDData());

            public static ItemMongoIDData Instance => Lazy.Value;

            public readonly RefHelper.PropertyRef<Item, object> RefTemplateId;

            private ItemMongoIDData()
            {
                RefTemplateId = RefHelper.PropertyRef<Item, object>.Create("TemplateId");
            }
        }

        public class ConditionMongoIDData
        {
            private static readonly Lazy<ConditionMongoIDData> Lazy =
                new Lazy<ConditionMongoIDData>(() => new ConditionMongoIDData());

            public static ConditionMongoIDData Instance => Lazy.Value;

            public readonly RefHelper.PropertyRef<Condition, object> RefId;

            private ConditionMongoIDData()
            {
                RefId = RefHelper.PropertyRef<Condition, object>.Create("id");
            }
        }
    }
}