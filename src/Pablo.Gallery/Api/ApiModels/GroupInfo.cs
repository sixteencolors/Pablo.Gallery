using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Pablo.Gallery.Models;

namespace Pablo.Gallery.Api.ApiModels {
    [DataContract(Name = "group")]
    public class GroupSummary {
        public Group Group { get; set; }

        public GroupSummary(Group group)
        {
            Group = group;
        }

        [DataMember(Name = "url")]
        public string Url { get { return "/group/" + Group.Slug; } set { } }

        [DataMember(Name = "name")]
        public string Name { get { return Group.Name; } set { } }

        [DataMember(Name = "slug")]
        public string Slug { get { return Group.Slug; } set { } }

    }

    [DataContract(Name = "group")]
    public class GroupDetail : GroupSummary {
        public GroupDetail(Group group, int page = 0, int size = Global.DefaultPageSize)
            : base(group) {            
            Packs = (from p in @group.Packs
                     select new PackSummary(p)).Skip(page * size).Take(size);
        }

        [DataMember(Name = "packs")]
        public IEnumerable<PackSummary> Packs { get; set; }
    }

    [DataContract(Name = "result")]
    public class GroupResult {
        [DataMember(Name = "groups")]
        public IEnumerable<GroupSummary> Groups { get; set; }
    }

    public class GroupMeta {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

}