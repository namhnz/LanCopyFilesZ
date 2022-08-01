﻿using System.Collections.Generic;

namespace CustomDialogs.Models.Imaging
{
    public abstract class ImageModel : ICustomFormattable
    {
        public virtual IReadOnlyCollection<string>? Formats { get; }

        public virtual bool AppendFormat(string formatInfo) => false;

        public abstract TImage? GetImage<TImage>() where TImage : class;
    }
}
