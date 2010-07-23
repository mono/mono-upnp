// 
// TextItemOptions.cs
//  
// Author:
//       Yavor Georgiev <fealebenpae@gmail.com>
// 
// Copyright (c) 2010 Yavor Georgiev
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
namespace Mono.Upnp.Dcp.MediaServer1.ContentDirectory1.Av
{
    public class TextItemOptions : ObjectOptions
    {
        public TextItemOptions ()
        {
            AuthorCollection = new List<PersonWithRole> ();
            PublisherCollection = new List<string> ();
            ContributorCollection = new List<string> ();
            RelationCollection = new List<Uri> ();
            RightsCollection = new List<string> ();
        }

        public TextItemOptions (TextItem textItem)
        {
            GetOptionsFrom (textItem);
        }

        protected override void GetOptionsFrom (Object obj)
        {
            var text_item = obj as TextItem;
            if (text_item != null)
            {
                Protection = text_item.Protection;
                LongDescription = text_item.LongDescription;
                Description = text_item.Description;
                StorageMedium = text_item.StorageMedium;
                Rating = text_item.Rating;
                Date = text_item.Date;
                Language = text_item.Language;

                AuthorCollection = new List<PersonWithRole> (text_item.Authors);
                PublisherCollection = new List<string> (text_item.Publishers);
                ContributorCollection = new List<string> (text_item.Contributors);
                RelationCollection = new List<Uri> (text_item.Relations);
                RightsCollection = new List<string> (text_item.Rights);
            }
            
            base.GetOptionsFrom (obj);
        }

        public virtual List<PersonWithRole> AuthorCollection { get; set; }

        public virtual string Protection { get; set; }

        public virtual string LongDescription { get; set; }

        public virtual string StorageMedium { get; set; }

        public virtual string Rating { get; set; }

        public virtual string Description { get; set; }

        public virtual List<string> PublisherCollection { get; set; }

        public virtual List<string> ContributorCollection { get; set; }

        public virtual string Date { get; set; }

        public virtual List<Uri> RelationCollection { get; set; }

        public virtual string Language { get; set; }

        public virtual List<string> RightsCollection { get; set; }
    }
}

