using HtmlAgilityPack;

namespace Travaloud.Tenants.SharedRCL.Utils;

public static class Helpers
{
    public static Dictionary<string, string> GetNationalities()
    {
        return new Dictionary<string, string>()
        {
            { "", "" },
            { "AF",  "Afghanistan" },
            { "AX", "Åland Islands" },
            { "AL", "Albania" },
            { "DZ", "Algeria" },
            { "AS", "American Samoa" },
            { "AD", "Andorra" },
            { "AO", "Angola" },
            { "AI", "Anguilla" },
            { "AQ", "Antarctica" },
            { "AG", "Antigua and Barbuda" },
            { "AR", "Argentina" },
            { "AM", "Armenia" },
            { "AW", "Aruba" },
            { "AU", "Australia" },
            { "AT", "Austria" },
            { "AZ", "Azerbaijan" },
            { "BS", "Bahamas" },
            { "BH", "Bahrain" },
            { "BD", "Bangladesh" },
            { "BB", "Barbados" },
            { "BY", "Belarus" },
            { "BE", "Belgium" },
            { "BZ", "Belize" },
            { "BJ", "Benin" },
            { "BM", "Bermuda" },
            { "BT", "Bhutan" },
            { "BO", "Bolivia" },
            { "BA", "Bosnia and Herzegovina" },
            { "BW", "Botswana" },
            { "BV", "Bouvet Island" },
            { "BR", "Brazil" },
            { "IO", "British Indian Ocean Territory" },
            { "VG", "British Virgin Islands" },
            { "BN", "Brunei" },
            { "BG", "Bulgaria" },
            { "BF", "Burkina Faso" },
            { "BI", "Burundi" },
            { "KH", "Cambodia" },
            { "CM", "Cameroon" },
            { "CA", "Canada" },
            { "CV", "Cape Verde" },
            { "BQ", "Caribbean Netherlands" },
            { "KY", "Cayman Islands" },
            { "CF", "Central African Republic" },
            { "TD", "Chad" },
            { "CL", "Chile" },
            { "CN", "China" },
            { "CX", "Christmas Island" },
            { "CC", "Cocos(Keeling) Islands" },
            { "CO", "Colombia" },
            { "KM", "Comoros" },
            { "CK", "Cook Islands" },
            { "CR", "Costa Rica" },
            { "CI", "Cote d\'Ivoire" },
            { "HR", "Croatia" },
            { "CU", "Cuba" },
            { "CW", "Curacao" },
            { "CY", "Cyprus" },
            { "CZ", "Czech Republic" },
            { "CD", "Democratic Republic of the Congo" },
            { "DK", "Denmark" },
            { "DJ", "Djibouti" },
            { "DM", "Dominica" },
            { "DO", "Dominican Republic" },
            { "EC", "Ecuador" },
            { "EG", "Egypt" },
            { "SV", "El Salvador" },
            { "GQ", "Equatorial Guinea" },
            { "ER", "Eritrea" },
            { "EE", "Estonia" },
            { "ET", "Ethiopia" },
            { "FK", "Falkland Islands (Islas Malvinas)" },
            { "FO", "Faroe Islands" },
            { "FJ", "Fiji" },
            { "FI", "Finland" },
            { "FR", "France" },
            { "GF", "French Guiana" },
            { "PF", "French Polynesia" },
            { "TF", "French Southern Territories" },
            { "GA", "Gabon" },
            { "GM", "Gambia" },
            { "GE", "Georgia" },
            { "DE", "Germany" },
            { "GH", "Ghana" },
            { "GI", "Gibraltar" },
            { "GR", "Greece" },
            { "GL", "Greenland" },
            { "GD", "Grenada" },
            { "GP", "Guadeloupe" },
            { "GU", "Guam" },
            { "GT", "Guatemala" },
            { "GG", "Guernsey" },
            { "GN", "Guinea" },
            { "GW", "Guinea - Bissau" },
            { "GY", "Guyana" },
            { "HT", "Haiti" },
            { "HM", "Heard & amp; McDonald Islands" },
            { "HN", "Honduras" },
            { "HK", "Hong Kong" },
            { "HU", "Hungary" },
            { "IS", "Iceland" },
            { "IN", "India" },
            { "ID", "Indonesia" },
            { "IR", "Iran" },
            { "IQ", "Iraq" },
            { "IE", "Ireland" },
            { "IM", "Isle of Man" },
            { "IL", "Israel" },
            { "IT", "Italy" },
            { "JM", "Jamaica" },
            { "JP", "Japan" },
            { "JE", "Jersey" },
            { "JO", "Jordan" },
            { "KZ", "Kazakhstan" },
            { "KE", "Kenya" },
            { "KI", "Kiribati" },
            { "XK", "Kosovo" },
            { "KW", "Kuwait" },
            { "KG", "Kyrgyzstan" },
            { "LA", "Laos" },
            { "LV", "Latvia" },
            { "LB", "Lebanon" },
            { "LS", "Lesotho" },
            { "LR", "Liberia" },
            { "LY", "Libya" },
            { "LI", "Liechtenstein" },
            { "LT", "Lithuania" },
            { "LU", "Luxembourg" },
            { "MO", "Macau SAR China" },
            { "MK", "Macedonia" },
            { "MG", "Madagascar" },
            { "MW", "Malawi" },
            { "MY", "Malaysia" },
            { "MV", "Maldives" },
            { "ML", "Mali" },
            { "MT", "Malta" },
            { "MH", "Marshall Islands" },
            { "MQ", "Martinique" },
            { "MR", "Mauritania" },
            { "MU", "Mauritius" },
            { "YT", "Mayotte" },
            { "MX", "Mexico" },
            { "FM", "Micronesia" },
            { "MD", "Moldova" },
            { "MC", "Monaco" },
            { "MN", "Mongolia" },
            { "ME", "Montenegro" },
            { "MS", "Montserrat" },
            { "MA", "Morocco" },
            { "MZ", "Mozambique" },
            { "MM", "Myanmar" },
            { "NA", "Namibia" },
            { "NR", "Nauru" },
            { "NP", "Nepal" },
            { "NL", "Netherlands" },
            { "AN", "Netherlands Antilles" },
            { "NC", "New Caledonia" },
            { "NZ", "New Zealand" },
            { "NI", "Nicaragua" },
            { "NE", "Niger" },
            { "NG", "Nigeria" },
            { "NU", "Niue" },
            { "NF", "Norfolk Island" },
            { "KP", "North Korea" },
            { "MP", "Northern Mariana Islands" },
            { "NO", "Norway" },
            { "OM", "Oman" },
            { "PK", "Pakistan" },
            { "PW", "Palau" },
            { "PS", "Palestinian Territory" },
            { "PA", "Panama" },
            { "PG", "Papua New Guinea" },
            { "PY", "Paraguay" },
            { "PE", "Peru" },
            { "PH", "Philippines" },
            { "PN", "Pitcairn Islands" },
            { "PL", "Poland" },
            { "PT", "Portugal" },
            { "PR", "Puerto Rico" },
            { "QA", "Qatar" },
            { "CG", "Republic of the Congo" },
            { "RE", "Reunion" },
            { "RO", "Romania" },
            { "RU", "Russia" },
            { "RW", "Rwanda" },
            { "KN", "Saint Kitts and Nevis" },
            { "LC", "Saint Lucia" },
            { "SX", "Saint Martin" },
            { "VC", "Saint Vincent and the Grenadines" },
            { "WS", "Samoa" },
            { "SM", "San Marino" },
            { "ST", "Sao Tome and Principe" },
            { "SA", "Saudi Arabia" },
            { "SN", "Senegal" },
            { "RS", "Serbia" },
            { "SC", "Seychelles" },
            { "SL", "Sierra Leone" },
            { "SG", "Singapore" },
            { "MF", "Sint Maarten" },
            { "SK", "Slovakia" },
            { "SI", "Slovenia" },
            { "SB", "Solomon Islands" },
            { "SO", "Somalia" },
            { "ZA", "South Africa" },
            { "GS", "South Georgia &amp; South Sandwich Islands" },
            { "KR", "South Korea" },
            { "SS", "South Sudan" },
            { "ES", "Spain" },
            { "LK", "Sri Lanka" },
            { "BL", "St.Barthélemy" },
            { "SH", "St.Helena" },
            { "PM", "St.Pierre & amp; Miquelon" },
            { "SD", "Sudan" },
            { "SR", "Suriname" },
            { "SJ", "Svalbard and Jan Mayen" },
            { "SZ", "Swaziland" },
            { "SE", "Sweden" },
            { "CH", "Switzerland" },
            { "SY", "Syria" },
            { "TW", "Taiwan" },
            { "TJ", "Tajikistan" },
            { "TZ", "Tanzania" },
            { "TH", "Thailand" },
            { "TL", "Timor - Leste" },
            { "TG", "Togo" },
            { "TK", "Tokelau" },
            { "TO", "Tonga" },
            { "TT", "Trinidad and Tobago" },
            { "TN", "Tunisia" },
            { "TR", "Turkey" },
            { "TM", "Turkmenistan" },
            { "TC", "Turks & amp; Caicos Islands" },
            { "TV", "Tuvalu" },
            { "UM", "U.S.Outlying Islands" },
            { "VI", "U.S.Virgin Islands" },
            { "UG", "Uganda" },
            { "UA", "Ukraine" },
            { "AE", "United Arab Emirates" },
            { "GB", "United Kingdom" },
            { "US", "United States of America" },
            { "UY", "Uruguay" },
            { "UZ", "Uzbekistan" },
            { "VU", "Vanuatu" },
            { "VA", "Vatican City" },
            { "VE", "Venezuela" },
            { "VN", "Vietnam" },
            { "WF", "Wallis and Futuna" },
            { "EH", "Western Sahara" },
            { "YE", "Yemen" },
            { "ZM", "Zambia" },
            { "ZW", "Zimbabwe" }

        };
    }

    public static List<List<T>> SplitList<T>(List<T> list)
    {
        var splitList = new List<List<T>>();

        if (list.Count == 1)
        {
            var subList = new List<T> {list[0]};
            splitList.Add(subList);
        }
        else
        {
            int numItemsToAdd;
            for (var i = 0; i < list.Count; i += numItemsToAdd)
            {
                numItemsToAdd = Math.Min(3, list.Count - i);

                if (numItemsToAdd == 1 && i != list.Count - 1)
                {
                    numItemsToAdd = 2;
                }

                var subList = new List<T>();

                for (var j = 0; j < numItemsToAdd; j++)
                {
                    if (!subList.Contains(list[i + j]))
                    {
                        subList.Add(list[i + j]);
                    }
                }

                if (subList.Count >= 2)
                {
                    splitList.Add(subList);
                }
                else
                {
                    // Merge with previous sub list
                    if (splitList.Count > 0)
                    {
                        splitList[^1].AddRange(subList);
                    }
                }
            }
        }

        return splitList;
    }

    public static List<List<CardComponent>> SplitCardsList(List<CardComponent> list)
    {
        var splitList = new List<List<CardComponent>>();

        var numOfLists = (int)Math.Ceiling(list.Count / 3.0);

        for (var i = 0; i < numOfLists; i++)
        {
            var subList = new List<CardComponent>();

            if (i == numOfLists - 1)
            {
                var numItemsToAdd = list.Count - i * 3;
                if (numItemsToAdd == 1)
                {
                    subList.Add(list[^1]);
                }
                else
                {
                    for (var j = 0; j < numItemsToAdd; j++)
                    {
                        subList.Add(list[i * 3 + j]);
                    }
                }
            }
            else
            {
                for (var j = 0; j < 3; j++)
                {
                    subList.Add(list[i * 3 + j]);
                }
            }

            splitList.Add(subList);
        }

        return splitList;
    }

    public static List<string> GetParagraphsListFromHtml(string sourceHtml)
    {
        //first create an HtmlDocument
        var doc = new HtmlDocument();

        //load the html (from a string)
        doc.LoadHtml(sourceHtml);

        //Select all the <p> nodes in a HtmlNodeCollection
        var paragraphs = doc.DocumentNode.SelectNodes(".//p");

        //Iterates on every Node in the collection

        return paragraphs.Select(paragraph => paragraph.InnerText).ToList();
    }
}