using System.Threading;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;

namespace Ysm.Data.Comments
{
   public class CommentService
    {
        public static async Task<CommentThread> GetCommentsAsync(string id)
        {
            CommentThread commentThread = new CommentThread();

            YouTubeService youTubeService = AuthenticationService.Default.YouTubeService;
            CommentThreadsResource.ListRequest request = youTubeService.CommentThreads.List("snippet");
            request.VideoId = id;
            request.MaxResults = 100;
            request.TextFormat = CommentThreadsResource.ListRequest.TextFormatEnum.PlainText;
            request.Order = CommentThreadsResource.ListRequest.OrderEnum.Time;
            

            CommentThreadListResponse result = await request.ExecuteAsync(CancellationToken.None);
            
            commentThread.NextPageToken = result.NextPageToken;

            foreach (Google.Apis.YouTube.v3.CommentThread thread in result.Items)
            {
                CommentSnippet snippet = thread.Snippet.TopLevelComment.Snippet;

                Comment comment = new Comment(thread.Snippet.TopLevelComment.Id)
                {
                    Text = snippet.TextDisplay,
                    Author = snippet.AuthorDisplayName,
                    ImageUrl = snippet.AuthorProfileImageUrl,
                    PublishedAt = snippet.PublishedAt,
                    Published = snippet.PublishedAtRaw
                };

                commentThread.Comments.Add(comment);
            }

            return commentThread;
        }
    }
}
