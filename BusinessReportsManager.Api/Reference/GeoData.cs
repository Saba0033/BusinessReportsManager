namespace BusinessReportsManager.Api.Reference;

/// <summary>
/// Static reference data for the Destination picker: a full country list plus a
/// curated set of major cities for popular travel destinations. Unknown countries
/// return no cities so the frontend can fall back to free-text entry.
/// </summary>
public static class GeoData
{
    public static readonly IReadOnlyList<string> Countries = new[]
    {
        "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Argentina", "Armenia",
        "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados",
        "Belarus", "Belgium", "Belize", "Benin", "Bhutan", "Bolivia", "Bosnia and Herzegovina",
        "Botswana", "Brazil", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia",
        "Cameroon", "Canada", "Cape Verde", "Central African Republic", "Chad", "Chile", "China",
        "Colombia", "Comoros", "Congo", "Costa Rica", "Croatia", "Cuba", "Cyprus", "Czech Republic",
        "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador",
        "Estonia", "Eswatini", "Ethiopia", "Fiji", "Finland", "France", "Gabon", "Gambia", "Georgia",
        "Germany", "Ghana", "Greece", "Grenada", "Guatemala", "Guinea", "Guyana", "Haiti", "Honduras",
        "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel", "Italy",
        "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kosovo", "Kuwait",
        "Kyrgyzstan", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein",
        "Lithuania", "Luxembourg", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta",
        "Mauritania", "Mauritius", "Mexico", "Moldova", "Monaco", "Mongolia", "Montenegro", "Morocco",
        "Mozambique", "Myanmar", "Namibia", "Nepal", "Netherlands", "New Zealand", "Nicaragua",
        "Niger", "Nigeria", "North Macedonia", "Norway", "Oman", "Pakistan", "Palestine", "Panama",
        "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Qatar",
        "Romania", "Russia", "Rwanda", "Saudi Arabia", "Senegal", "Serbia", "Seychelles",
        "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Somalia", "South Africa", "South Korea",
        "Spain", "Sri Lanka", "Sudan", "Suriname", "Sweden", "Switzerland", "Syria", "Taiwan",
        "Tajikistan", "Tanzania", "Thailand", "Togo", "Tonga", "Trinidad and Tobago", "Tunisia",
        "Turkey", "Turkmenistan", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom",
        "United States", "Uruguay", "Uzbekistan", "Vanuatu", "Vatican City", "Venezuela", "Vietnam",
        "Yemen", "Zambia", "Zimbabwe"
    };

    public static readonly IReadOnlyDictionary<string, string[]> CitiesByCountry =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Georgia"] = new[] { "Tbilisi", "Batumi", "Kutaisi", "Bakuriani", "Gudauri", "Mestia", "Kazbegi", "Telavi", "Borjomi" },
            ["Italy"] = new[] { "Rome", "Milan", "Venice", "Florence", "Naples", "Verona", "Bologna", "Turin", "Pisa", "Rimini" },
            ["France"] = new[] { "Paris", "Nice", "Lyon", "Marseille", "Cannes", "Bordeaux", "Strasbourg", "Toulouse" },
            ["Spain"] = new[] { "Madrid", "Barcelona", "Seville", "Valencia", "Malaga", "Granada", "Bilbao", "Palma" },
            ["Greece"] = new[] { "Athens", "Thessaloniki", "Santorini", "Mykonos", "Crete", "Rhodes", "Corfu" },
            ["Turkey"] = new[] { "Istanbul", "Antalya", "Ankara", "Bodrum", "Izmir", "Cappadocia", "Trabzon" },
            ["United Arab Emirates"] = new[] { "Dubai", "Abu Dhabi", "Sharjah", "Ras Al Khaimah" },
            ["United Kingdom"] = new[] { "London", "Edinburgh", "Manchester", "Liverpool", "Glasgow", "Birmingham", "Oxford" },
            ["United States"] = new[] { "New York", "Los Angeles", "Las Vegas", "Miami", "Orlando", "San Francisco", "Chicago", "Washington" },
            ["Germany"] = new[] { "Berlin", "Munich", "Frankfurt", "Hamburg", "Cologne", "Düsseldorf" },
            ["Austria"] = new[] { "Vienna", "Salzburg", "Innsbruck", "Graz" },
            ["Switzerland"] = new[] { "Zurich", "Geneva", "Lucerne", "Interlaken", "Bern", "Zermatt" },
            ["Netherlands"] = new[] { "Amsterdam", "Rotterdam", "The Hague", "Utrecht" },
            ["Czech Republic"] = new[] { "Prague", "Brno", "Karlovy Vary" },
            ["Egypt"] = new[] { "Cairo", "Hurghada", "Sharm El Sheikh", "Luxor", "Alexandria" },
            ["Thailand"] = new[] { "Bangkok", "Phuket", "Pattaya", "Chiang Mai", "Krabi" },
            ["Maldives"] = new[] { "Malé" },
            ["Japan"] = new[] { "Tokyo", "Osaka", "Kyoto", "Hokkaido", "Nagoya" },
            ["China"] = new[] { "Beijing", "Shanghai", "Guangzhou", "Xian", "Hong Kong" },
            ["India"] = new[] { "Delhi", "Mumbai", "Goa", "Jaipur", "Agra", "Bangalore" },
            ["Azerbaijan"] = new[] { "Baku", "Gabala", "Sheki", "Ganja" },
            ["Armenia"] = new[] { "Yerevan", "Gyumri", "Dilijan", "Tsaghkadzor" },
            ["Russia"] = new[] { "Moscow", "Saint Petersburg", "Sochi", "Kazan" },
            ["Qatar"] = new[] { "Doha" },
            ["Portugal"] = new[] { "Lisbon", "Porto", "Faro", "Madeira" },
            ["Cyprus"] = new[] { "Limassol", "Larnaca", "Paphos", "Nicosia", "Ayia Napa" },
            ["Israel"] = new[] { "Tel Aviv", "Jerusalem", "Eilat", "Haifa" }
        };
}
