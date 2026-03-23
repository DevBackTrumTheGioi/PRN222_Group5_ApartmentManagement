namespace PRN222_ApartmentManagement.Utils;

public static class UsernameGenerator
{
    private static readonly Random _random = new();

    private static readonly Dictionary<char, char> _vietnameseMap = new()
    {
        {'à','a'},{'á','a'},{'ả','a'},{'ã','a'},{'ạ','a'},
        {'ă','a'},{'ằ','a'},{'ắ','a'},{'ẳ','a'},{'ẵ','a'},{'ặ','a'},
        {'â','a'},{'ầ','a'},{'ấ','a'},{'ẩ','a'},{'ẫ','a'},{'ậ','a'},
        {'è','e'},{'é','e'},{'ẻ','e'},{'ẽ','e'},{'ẹ','e'},
        {'ê','e'},{'ề','e'},{'ế','e'},{'ể','e'},{'ễ','e'},{'ệ','e'},
        {'ì','i'},{'í','i'},{'ỉ','i'},{'ĩ','i'},{'ị','i'},
        {'ò','o'},{'ó','o'},{'ỏ','o'},{'õ','o'},{'ọ','o'},
        {'ô','o'},{'ồ','o'},{'ố','o'},{'ổ','o'},{'ỗ','o'},{'ộ','o'},
        {'ơ','o'},{'ờ','o'},{'ớ','o'},{'ở','o'},{'ỡ','o'},{'ợ','o'},
        {'ù','u'},{'ú','u'},{'ủ','u'},{'ũ','u'},{'ụ','u'},
        {'ư','u'},{'ừ','u'},{'ứ','u'},{'ử','u'},{'ữ','u'},{'ự','u'},
        {'ỳ','y'},{'ý','y'},{'ỷ','y'},{'ỹ','y'},{'ỵ','y'},
        {'đ','d'},
        {'À','A'},{'Á','A'},{'Ả','A'},{'Ã','A'},{'Ā','A'},{'Ạ','A'},
        {'Ă','A'},{'Ằ','A'},{'Ắ','A'},{'Ẳ','A'},{'Ẵ','A'},{'Ặ','A'},
        {'Â','A'},{'Ầ','A'},{'Ấ','A'},{'Ẩ','A'},{'Ẫ','A'},{'Ậ','A'},
        {'È','E'},{'É','E'},{'Ẻ','E'},{'Ẽ','E'},{'Ẹ','E'},
        {'Ê','E'},{'Ề','E'},{'Ế','E'},{'Ể','E'},{'Ễ','E'},{'Ệ','E'},
        {'Ì','I'},{'Í','I'},{'Ỉ','I'},{'Ĩ','I'},{'Ị','I'},
        {'Ò','O'},{'Ó','O'},{'Ỏ','O'},{'Õ','O'},{'Ọ','O'},
        {'Ô','O'},{'Ồ','O'},{'Ố','O'},{'Ổ','O'},{'Ỗ','O'},{'Ộ','O'},
        {'Ơ','O'},{'Ờ','O'},{'Ớ','O'},{'Ở','O'},{'Ỡ','O'},{'Ợ','O'},
        {'Ù','U'},{'Ú','U'},{'Ủ','U'},{'Ũ','U'},{'Ụ','U'},
        {'Ư','U'},{'Ừ','U'},{'Ứ','U'},{'Ử','U'},{'Ữ','U'},{'Ự','U'},
        {'Ỳ','Y'},{'Ý','Y'},{'Ỷ','Y'},{'Ỹ','Y'},{'Ỵ','Y'},
        {'Đ','D'}
    };

    public static string RemoveVietnameseMarks(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        var sb = new System.Text.StringBuilder(input.Length);
        foreach (var c in input)
        {
            sb.Append(_vietnameseMap.TryGetValue(c, out var replacement) ? replacement : c);
        }
        return sb.ToString();
    }

    public static string Generate(string fullName)
    {
        var cleanName = RemoveVietnameseMarks(fullName);
        var words = cleanName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length < 2)
            throw new ArgumentException("FullName must have at least 2 words.");

        var lastName = words[^1].ToUpper();
        var middleInitials = string.Concat(words.Take(words.Length - 1).Select(w => w[0]));
        var digits = _random.Next(1000, 9999);

        return $"{lastName}{middleInitials}{digits}";
    }
}
