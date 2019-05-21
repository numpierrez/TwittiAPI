using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.Owin.Security;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using System.Linq;
using System.Collections;
using Tweetinvi.Logic;

namespace Twitti.Controllers
{

    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
            // var userCredentials = Auth.CreateCredentials("FMUFYf7B9a6IL7qy0ap4jOVEZ ", "3GOfpeRsRxSQLRUVsbnRpATJUb6FsFQMTZK3kzlIcApXIdZEc4 ", "1125366856003596288-u9zKklFe7wpIvpj0ILo3jmvTkq0HoK", "y1jPqWx6jIfRwTBm7QckVNdLC0nsDqtye9BG5DVdwr0bQ");
            // var authenticatedUser = Tweetinvi.User.GetAuthenticatedUser(userCredentials);
            //var firstTweet = Tweet.PublishTweet("I love Tweetinvi!");
            Tweetinvi.Auth.SetUserCredentials("FMUFYf7B9a6IL7qy0ap4jOVEZ", "3GOfpeRsRxSQLRUVsbnRpATJUb6FsFQMTZK3kzlIcApXIdZEc4", "1125366856003596288-TbKIyBjobq02HPSeilLIk34I6UCKlM", "AsSNeypsUZCCHxhQFhstfYfnpN32ayXE5yUikxDNyPzDd");
        }

        [HttpGet]
        public bool Twit(string twit) {

            var postTweet = Tweetinvi.Tweet.PublishTweet(twit);
            if (postTweet != null)
                return true;
            else
                return false;
        }

        [HttpGet]
        public HttpResponseMessage conseguirSeguidores(string conseguirSeguidores)
        {
            long[] paises = new long[13];
            paises[0] = 23424768;
            paises[1] = 23424782;
            paises[2] = 23424787;
            paises[3] = 23424801;
            paises[4] = 23424919;
            paises[5] = 23424768;
            paises[6] = 23424762;
            paises[7] = 468052;
            paises[8] = 23424762;
            paises[9] = 1;
            paises[10] = 23424950;
            paises[11] = 615702;
            paises[12] = 44418;
            IAuthenticatedUser usAut = Tweetinvi.User.GetAuthenticatedUser();
            foreach (var woeid in paises)
            {
                var trends = Trends.GetTrendsAt(woeid);
                if (trends != null)
                {
                    var tweets = Search.SearchTweets(trends.Trends[0].Name);
                    var tweets2 = Search.SearchTweets(trends.Trends[1].Name);
                    var tweets3 = Search.SearchTweets(trends.Trends[2].Name);

                    if (tweets != null)
                    {
                        
                        usAut.FollowUser(tweets.First().CreatedBy.ScreenName);
                    }
                    if (tweets2 != null)
                    {
                        usAut.FollowUser(tweets2.First().CreatedBy.ScreenName);
                    }
                    if (tweets3 != null)
                    {
                        usAut.FollowUser(tweets3.First().CreatedBy.ScreenName);
                    }

                }
            }
            return null;
        }
        [HttpGet]
        public HttpResponseMessage twitearTendencia(string tendencia2)
        {
            var tweets = Search.SearchTweets(tendencia2);
            int i = 0;
            if (tweets != null)
            {
                foreach (var twi in tweets)
                {
                    if (twi.FullText.StartsWith("RT"))
                    {
                        var postTweet = Tweetinvi.Tweet.PublishRetweet(twi);
                    }
                    else
                    {
                        var postTweet = Tweetinvi.Tweet.PublishTweet(twi.FullText);
                    }
                    i++;
                    if (i > 15)
                        break;
                }
            }
            return null;
        }

        [HttpGet]
        public HttpResponseMessage getFollowers(string followers) {
            List<user> usuario = new List<user>();
            
            IAuthenticatedUser usAut = Tweetinvi.User.GetAuthenticatedUser();
            var fllowers = usAut.GetFollowers();
            if (fllowers != null)
            {
                foreach (var us in fllowers)
                {
                    user friend = new user();
                    friend.usario = us.Name;
                    friend.imagen = us.ProfileImageUrl400x400;
                    friend.followers = us.FollowersCount;
                    friend.follow = us.FriendsCount;
                    friend.nick = us.UserIdentifier.ScreenName;
                    friend.descripcion = us.Description;
                    usuario.Add(friend);
                }
                return Request.CreateResponse(usuario);
            }
            else
                return null;
        }

        [HttpGet]
        public HttpResponseMessage getFollows(string follows)
        {
            List<user> usuario = new List<user>();

            IAuthenticatedUser usAut = Tweetinvi.User.GetAuthenticatedUser();
            var fllows = usAut.GetFriends();
            if (fllows != null)
            {
                foreach (var us in fllows)
                {
                    user friend = new user();
                    friend.usario = us.Name;
                    friend.nick = us.UserIdentifier.ScreenName;
                    friend.imagen = us.ProfileImageUrl400x400;
                    friend.followers = us.FollowersCount;
                    friend.follow = us.FriendsCount;
                    friend.descripcion = us.Description;
                    usuario.Add(friend);
                }
                return Request.CreateResponse(usuario);
            }
            else
                return null;
        }

        [HttpGet]
        public HttpResponseMessage unfollow(string unfollow)
        {
            List<long> followList = new List<long>();
            List<long> followersList = new List<long>();
            IAuthenticatedUser usAut = Tweetinvi.User.GetAuthenticatedUser();

            var fllowers = usAut.GetFollowers();
            if (fllowers != null)
            {
                foreach (var us in fllowers)
                {
                    var nick = us.UserIdentifier.Id;
                    followList.Add(nick);
                }
            }
            var follows = usAut.GetFriends();
            if (follows != null)
            {
                foreach (var us in follows)
                {
                    var nick2 = us.UserIdentifier.Id;
                    followersList.Add(nick2);
                }
            }
            List<long> result = new List<long>();
            result = followersList.Except(followList).ToList();

            int i = 0;
            foreach (var wat in result) {
                if (i > 10)
                    break;
                usAut.UnFollowUser(wat);
            }
            return null;
        }
        [HttpGet]
        public HttpResponseMessage GetUser(string token)
        {
            user usuario = new user();

            IAuthenticatedUser usAut = Tweetinvi.User.GetAuthenticatedUser();
            
            usuario.nick = usAut.ScreenName;
            
            usuario.usario = usAut.Name;
            usuario.imagen = usAut.ProfileImageUrl400x400;
            usuario.followers = usAut.FollowersCount;
            usuario.follow = usAut.FriendsCount;
            usuario.descripcion = usAut.Description;
            var tweets = Timeline.GetUserTimeline(usAut.UserIdentifier);
            if (tweets != null)
            {
                List<tw> timeLineHistoric = new List<tw>();
                foreach (var twi in tweets)
                {
                    tw twiti = new tw();
                    int i = 0;
                    if (twi.IsRetweet)
                    {
                        twiti.text = twi.RetweetedTweet.FullText;
                        twiti.likes = twi.RetweetedTweet.FavoriteCount;
                        twiti.retwets = twi.RetweetedTweet.RetweetCount;
                        twiti.usario = twi.RetweetedTweet.CreatedBy.Name;
                        twiti.imagen = twi.RetweetedTweet.CreatedBy.ProfileImageUrl400x400;
                        twiti.nick = twi.RetweetedTweet.CreatedBy.UserIdentifier.ScreenName;

                        timeLineHistoric.Add(twiti);
                    }
                    else
                    {
                        twiti.text = twi.FullText;
                        twiti.likes = twi.FavoriteCount;
                        twiti.retwets = twi.RetweetCount;
                        twiti.usario = twi.CreatedBy.Name;
                        twiti.imagen = twi.CreatedBy.ProfileImageUrl400x400;
                        twiti.nick = twi.CreatedBy.UserIdentifier.ScreenName;
                        timeLineHistoric.Add(twiti);
                    }
                    i++;
                    if (i > 100)
                        break;

                }
                usuario.timeline = timeLineHistoric;
            }
            return Request.CreateResponse(usuario);
        }
        [HttpGet]
        public bool postTrend(string token2)
        {
            long[] paises = new long[20];
            paises[0] = 23424768;
            paises[1] = 23424782;
            paises[2] = 23424787;
            paises[3] = 23424801;
            paises[4] = 23424919;
            paises[5] = 23424768;
            paises[6] = 23424762;
            paises[7] = 468052;
            paises[8] = 23424762;
            paises[9] = 1;
            paises[10] = 4;
            paises[11] = 123;
            paises[12] = 2;
            paises[13] = 3;
            paises[14] = 5;
            paises[15] = 6;
            paises[16] = 7;
            paises[17] = 8;
            paises[18] = 9;
            paises[19] = 1;

            foreach (var woeid in paises) {
                var trends = Trends.GetTrendsAt(woeid);
                if (trends != null)
                {
                    var tweets = Search.SearchTweets(trends.Trends[0].Name);
                    var tweets2 = Search.SearchTweets(trends.Trends[1].Name);

                    if (tweets != null)
                    {
                        if (tweets.First().FullText.StartsWith("RT"))
                        {
                            var postTweet = Tweetinvi.Tweet.PublishRetweet(tweets.First());
                        }
                        else
                        {
                            var postTweet = Tweetinvi.Tweet.PublishTweet(tweets.First().FullText);
                        }
                    }
                    if (tweets2 != null)
                    {
                        if (tweets2.First().FullText.StartsWith("RT"))
                        {
                            var postTweet2 = Tweetinvi.Tweet.PublishRetweet(tweets2.First());
                        }
                        else
                        {
                            var postTweet2 = Tweetinvi.Tweet.PublishTweet(tweets2.First().FullText);
                        }
                    }

                }
            }
            return true;
        }

        [HttpGet]
        public HttpResponseMessage Get(string location)
        {
            //Tweetinvi.Auth.
            if (location == "")
             location = "Argentina";
            long woeid = 468052;
            switch (location) {
                case "Argentina":
                    woeid = 23424747;
                    break;
                case "Brasil":
                    woeid = 23424768;
                    break;
                case "Chile":
                    woeid = 23424782;
                    break;
                case "Colombia":
                    woeid = 23424787;
                    break;
                case "Ecuador":
                    woeid = 23424801;
                    break;
                case "Peru":
                    woeid = 23424919;
                    break;
                case "Bolivia":
                    woeid = 23424762;
                    break;
                case "Uruguay":
                    woeid = 468052;
                    break;
                case "Paraguay":
                    woeid = 23424762;
                    break;
                case "Us":
                    woeid = 1;
                    break;
                case "España":
                    woeid = 23424950;
                    break;
                case "Francia":
                    woeid = 615702;
                    break;
                case "Inglaterra":
                    woeid = 44418;
                    break;
                case "Venezuela":
                    woeid = 23424762;
                    break;
            }

            
            var trends = Trends.GetTrendsAt(woeid);
            
            List<String> tendencias = new List<string>();
            int i = 1;

            if (trends != null)
            {
                foreach (var ten in trends.Trends)
                {
                    tendencias.Add(ten.Name);
                    i++;
                    if (i > 20)
                        break;
                }
            }
            else
                tendencias.Add("No hay tendencias para ese Pais");
             return Request.CreateResponse(tendencias);
        }


        [HttpGet]
        public HttpResponseMessage tweetsPorTendencia(string tendencia){
            List<tw> respuesta = new List<tw>();
            var tweets = Search.SearchTweets(tendencia);
            int i = 0;
            if (tweets != null)
            {
                foreach (var twi in tweets)
                {
                    tw twiti = new tw();
                    if (twi.IsRetweet)
                    {
                        twiti.text = twi.RetweetedTweet.FullText;
                        twiti.likes = twi.RetweetedTweet.FavoriteCount;
                        twiti.retwets = twi.RetweetedTweet.RetweetCount;
                        twiti.usario = twi.RetweetedTweet.CreatedBy.Name;
                        twiti.imagen = twi.RetweetedTweet.CreatedBy.ProfileImageUrl400x400;
                        twiti.nick = twi.RetweetedTweet.CreatedBy.UserIdentifier.ScreenName;

                        respuesta.Add(twiti);
                    }
                    else
                    {
                        twiti.text = twi.FullText;
                        twiti.likes = twi.FavoriteCount;
                        twiti.retwets = twi.RetweetCount;
                        twiti.usario = twi.CreatedBy.Name;
                        twiti.imagen = twi.CreatedBy.ProfileImageUrl400x400;
                        twiti.nick = twi.CreatedBy.UserIdentifier.ScreenName;
                        respuesta.Add(twiti);
                    }
                   
                    i++;
                    if (i > 40)
                        break;
                }
                return Request.CreateResponse(respuesta);
            }
            else
                return null;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
        public class tw {
            public string usario { get; set; }
            public string nick { get; set; }
            public string imagen { get; set; }
            public string imagenTwit { get; set; }
            public string text { get; set; }
            public long likes { get; set; }
            public long retwets { get; set; }
        }
        public class user
        {
            public string usario { get; set; }
            public string nick { get; set; }
            public string descripcion { get; set; }
            public string imagen { get; set; }
            public long follow { get; set; }
            public long followers { get; set; }
            public List<tw> timeline { get; set; }
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }


        

       

        #endregion
    }
}
