using System;
using System.Collections.Generic;
using System.Text;

namespace SeattleCollectionCalendarWrapper
{
    /// <summary>
    /// Representing a date collection occurs
    /// </summary>
    public class CollectionDate
    {
        /// <summary>
        /// Doesn't seem to be used, always 0
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Contains the Html code(utf-8 encoded) for the calendar collection icons
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Date for this collection instance
        /// </summary>
        public DateTimeOffset Start { get; set; }

        /// <summary>
        /// Doesn't seem to be used, always null
        /// </summary>
        public DateTimeOffset? End { get; set; }

        /// <summary>
        /// Doesn't seem to be used, always null
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Unknown, seems to always be true
        /// </summary>
        public bool AllDay { get; set; }

        /// <summary>
        /// Food and yard waste are collected on this date
        /// </summary>
        public bool FoodAndYardWaste { get; set; }

        /// <summary>
        /// Garbage is collected on this date
        /// </summary>
        public bool Garbage { get; set; }

        /// <summary>
        /// Recycling is collected on this date
        /// </summary>
        public bool Recycling { get; set; }

        /// <summary>
        /// doesn't seem to be used, always null
        /// </summary>
        public object DelimitedData { get; set; }

        /// <summary>
        /// Used when request is invalid, null on success
        /// </summary>
        public object Status { get; set; }

    }
}
