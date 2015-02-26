﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NuGet.Versioning.Test
{
    public class FloatingRangeTests
    {
        [Theory]
        [InlineData("1.0.0")]
        public void FloatRange_ParseBasic(string version)
        {
            FloatRange range = FloatRange.Parse(version);

            Assert.Equal(range.MinVersion, range.MinVersion);
            Assert.Equal(range.FloatBehavior, NuGetVersionFloatBehavior.None);
        }

        [Fact]
        public void FloatRange_ParsePrerelease()
        {
            FloatRange range = FloatRange.Parse("1.0.0-*");

            Assert.True(range.Satisfies(NuGetVersion.Parse("1.0.0-alpha")));
            Assert.True(range.Satisfies(NuGetVersion.Parse("1.0.0-beta")));
            Assert.True(range.Satisfies(NuGetVersion.Parse("1.0.0")));

            Assert.False(range.Satisfies(NuGetVersion.Parse("1.0.1-alpha")));
            Assert.False(range.Satisfies(NuGetVersion.Parse("1.0.1")));
        }

        [Fact]
        public void FloatingRange_FloatNone()
        {
            FloatRange range = FloatRange.Parse("1.0.0");

            Assert.Equal("1.0.0", range.MinVersion.ToNormalizedString());
            Assert.Equal(NuGetVersionFloatBehavior.None, range.FloatBehavior);
        }

        [Fact]
        public void FloatingRange_FloatPre()
        {
            FloatRange range = FloatRange.Parse("1.0.0-*");

            Assert.Equal("1.0.0--", range.MinVersion.ToNormalizedString());
            Assert.Equal(NuGetVersionFloatBehavior.Prerelease, range.FloatBehavior);
        }

        [Fact]
        public void FloatingRange_FloatPrePrefix()
        {
            FloatRange range = FloatRange.Parse("1.0.0-alpha-*");

            Assert.Equal("1.0.0-alpha-", range.MinVersion.ToNormalizedString());
            Assert.Equal(NuGetVersionFloatBehavior.Prerelease, range.FloatBehavior);
        }

        [Fact]
        public void FloatingRange_FloatRev()
        {
            FloatRange range = FloatRange.Parse("1.0.0.*");

            Assert.Equal("1.0.0", range.MinVersion.ToNormalizedString());
            Assert.Equal(NuGetVersionFloatBehavior.Revision, range.FloatBehavior);
        }

        [Fact]
        public void FloatingRange_FloatPatch()
        {
            FloatRange range = FloatRange.Parse("1.0.*");

            Assert.Equal("1.0.0", range.MinVersion.ToNormalizedString());
            Assert.Equal(NuGetVersionFloatBehavior.Patch, range.FloatBehavior);
        }

        [Fact]
        public void FloatingRange_FloatMinor()
        {
            FloatRange range = FloatRange.Parse("1.*");

            Assert.Equal("1.0.0", range.MinVersion.ToNormalizedString());
            Assert.Equal(NuGetVersionFloatBehavior.Minor, range.FloatBehavior);
        }

        [Fact]
        public void FloatingRange_FloatMajor()
        {
            FloatRange range = FloatRange.Parse("*");

            Assert.Equal("0.0.0", range.MinVersion.ToNormalizedString());
            Assert.Equal(NuGetVersionFloatBehavior.Major, range.FloatBehavior);
        }


        [Fact]
        public void FloatingRange_FloatNoneBest()
        {
            VersionRange range = VersionRange.Parse("1.0.0");

            List<NuGetVersion> versions = new List<NuGetVersion>()
            {
                NuGetVersion.Parse("1.0.0"),
                NuGetVersion.Parse("1.0.1"),
                NuGetVersion.Parse("2.0.0"),
            };

            Assert.Equal("1.0.0", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Fact]
        public void FloatingRange_FloatMinorBest()
        {
            VersionRange range = VersionRange.Parse("1.*");

            List<NuGetVersion> versions = new List<NuGetVersion>()
            {
                NuGetVersion.Parse("0.1.0"),
                NuGetVersion.Parse("1.0.0"),
                NuGetVersion.Parse("1.2.0"),
                NuGetVersion.Parse("2.0.0"),
            };

            Assert.Equal("1.2.0", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Fact]
        public void FloatingRange_FloatMinorPrefixNotFoundBest()
        {
            VersionRange range = VersionRange.Parse("1.*");

            List<NuGetVersion> versions = new List<NuGetVersion>()
            {
                NuGetVersion.Parse("0.1.0"),
                NuGetVersion.Parse("2.0.0"),
                NuGetVersion.Parse("2.5.0"),
                NuGetVersion.Parse("3.3.0"),
            };

            // take the nearest when the prefix is not matched
            Assert.Equal("2.0.0", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Fact]
        public void FloatingRange_FloatAllBest()
        {
            VersionRange range = VersionRange.Parse("*");

            List<NuGetVersion> versions = new List<NuGetVersion>()
            {
                NuGetVersion.Parse("0.1.0"),
                NuGetVersion.Parse("2.0.0"),
                NuGetVersion.Parse("2.5.0"),
                NuGetVersion.Parse("3.3.0"),
            };

            Assert.Equal("3.3.0", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Fact]
        public void FloatingRange_FloatPrereleaseBest()
        {
            VersionRange range = VersionRange.Parse("1.0.0-*");

            List<NuGetVersion> versions = new List<NuGetVersion>()
            {
                NuGetVersion.Parse("0.1.0-alpha"),
                NuGetVersion.Parse("1.0.0-alpha01"),
                NuGetVersion.Parse("1.0.0-alpha02"),
                NuGetVersion.Parse("2.0.0-beta"),
                NuGetVersion.Parse("2.0.1"),
            };

            Assert.Equal("1.0.0-alpha02", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Fact]
        public void FloatingRange_FloatPrereleaseNotFoundBest()
        {
            // "1.0.0-*"
            VersionRange range = VersionRange.Parse("1.0.0-*");

            List<NuGetVersion> versions = new List<NuGetVersion>()
            {
                NuGetVersion.Parse("0.1.0-alpha"),
                NuGetVersion.Parse("1.0.1-alpha01"),
                NuGetVersion.Parse("1.0.1-alpha02"),
                NuGetVersion.Parse("2.0.0-beta"),
                NuGetVersion.Parse("2.0.1"),
            };

            Assert.Equal("1.0.1-alpha01", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Fact]
        public void FloatingRange_FloatPrereleasePartialBest()
        {
            VersionRange range = VersionRange.Parse("1.0.0-alpha*");

            List<NuGetVersion> versions = new List<NuGetVersion>()
            {
                NuGetVersion.Parse("0.1.0-alpha"),
                NuGetVersion.Parse("1.0.0-alpha01"),
                NuGetVersion.Parse("1.0.0-alpha02"),
                NuGetVersion.Parse("2.0.0-beta"),
                NuGetVersion.Parse("2.0.1"),
            };

            Assert.Equal("1.0.0-alpha02", range.FindBestMatch(versions).ToNormalizedString());
        }
    }
}