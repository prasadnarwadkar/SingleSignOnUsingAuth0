using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication.ViewModels
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class MatchingPV
    {
        public string propertyId { get; set; }
        public string label { get; set; }
        public double score { get; set; }
        public bool important { get; set; }
    }

    public class DestinationEntity
    {
        public string id { get; set; }
        public string title { get; set; }
        public string stemId { get; set; }
        public bool isLeaf { get; set; }
        public int postcoordinationAvailability { get; set; }
        public bool hasCodingNote { get; set; }
        public bool hasMaternalChapterLink { get; set; }
        public List<MatchingPV> matchingPVs { get; set; }
        public bool propertiesTruncated { get; set; }
        public bool isResidualOther { get; set; }
        public bool isResidualUnspecified { get; set; }
        public string chapter { get; set; }
        public object theCode { get; set; }
        public double score { get; set; }
        public bool titleIsASearchResult { get; set; }
        public bool titleIsTopScore { get; set; }
        public int entityType { get; set; }
        public bool important { get; set; }
        public List<object> descendants { get; set; }
    }

    public class Root
    {
        public bool error { get; set; }
        public object errorMessage { get; set; }
        public bool resultChopped { get; set; }
        public bool wordSuggestionsChopped { get; set; }
        public int guessType { get; set; }
        public string uniqueSearchId { get; set; }
        public object words { get; set; }
        public List<DestinationEntity> destinationEntities { get; set; }
    }


}