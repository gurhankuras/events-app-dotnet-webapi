public class EmailHtmlTemplates {
    public static string ConfirmEmail(string confirmLink)
    {
        return $@"
        <div>
            Sayın Gurhan,<br>
            Lütfen aşağıdaki linke tıklayarak yeni açmış olduğunuz hesabı doğrulayınız.<br><br>
            
            <a href='{confirmLink}'>{confirmLink}</a><br><br>
            
            Eğer bu hesabı siz açmamışsanız yukarıda verilmiş olan linke tıklamayınız.
        </div>
        ";
    }    
}