namespace Demo.Base
{
    public class TestOwnedType
    {
        public static IEnumerable<Article> BuildArticlesForInsert()
        {
            for(int i=0;i<30;i++)
            {
                Article a1 = new Article();
                a1.Content = "Content"+i;
                a1.Remarks = new MultiString { Chinese = "我是中国人" + i, English = "I am Chinese" + i };
                a1.Title = "Title" + i;
                yield return a1;
            }
        }

        public static Task RunAsync(BaseDbContext ctx)
        {
            Article a1 = new Article();
            a1.Content = "Content111";
            a1.Remarks = new MultiString { Chinese="我是中国人",English="I am Chinese"};
            a1.Title = "Title 666";
            ctx.Articles.Add(a1);
            return ctx.SaveChangesAsync();
        }
    }
}
