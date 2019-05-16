using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using SocialNetWork.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SocialNetwork.Models;
using SocialNetwork.Services;

namespace SocialNetwork
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SeedData(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddScoped<UserService>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<CommentService>();
            services.AddScoped<PostService>();
            services.AddScoped<WallService>();
            services.AddScoped<UserService>();
            services.AddScoped<LoginTestService>();
            services.AddScoped<CircleService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();
            app.UseSession();

            app.UseAuthentication();
            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Post}/{action=Index}/{id?}");
            });

            
        }




        private void SeedData(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetworkDb"));
            client.DropDatabase("SocialNetworkDb");
            var database = client.GetDatabase("SocialNetworkDb");

            var userCollec = database.GetCollection<User>("Users");
            var wallCollec = database.GetCollection<Wall>("Walls");
            var circleCollec = database.GetCollection<Circle>("circles");
            var postCollec = database.GetCollection<Post>("Posts");
            var commentCollec = database.GetCollection<Comment>("Comments");

            //adding users
            User u1 = new User(){Password = "1234", UserName = "u1", Name = "u1"};
            User u2 = new User() {Password = "1234", UserName = "u2", Name = "u2" };
            User u3 = new User() { Password = "1234", UserName = "u3", Name = "u3" };
            User u4 = new User() {Password = "1234", UserName = "u4", Name = "u4"};
            User u5 = new User() { Password = "1234", UserName = "u5", Name = "u5" };

            userCollec.InsertOne(u1);
            userCollec.InsertOne(u2);
            userCollec.InsertOne(u3);
            userCollec.InsertOne(u4);
            userCollec.InsertOne(u5);

            //Adding user walls
            Wall wu1 = new Wall(){owner = u1.Name,ownerID = u1.Id,type = "User"};
            Wall wu2 = new Wall() {owner = u2.Name, ownerID = u2.Id, type = "User" };
            Wall wu3 = new Wall() {owner = u3.Name, ownerID = u3.Id, type = "User" };
            Wall wu4 = new Wall() {owner = u4.Name, ownerID = u4.Id, type = "User" };
            Wall wu5 = new Wall() {owner = u5.Name, ownerID = u5.Id, type = "User" };

            wallCollec.InsertOne(wu1);
            wallCollec.InsertOne(wu2);
            wallCollec.InsertOne(wu3);
            wallCollec.InsertOne(wu4);
            wallCollec.InsertOne(wu5);

            //adding walls to users
            u1.Wall = wu1.ID;
            u2.Wall = wu2.ID;
            u3.Wall = wu3.ID;
            u4.Wall = wu4.ID;
            u5.Wall = wu5.ID;
            userCollec.ReplaceOne<User>(u => u.Id == u1.Id, u1);
            userCollec.ReplaceOne<User>(u => u.Id == u2.Id, u2);
            userCollec.ReplaceOne<User>(u => u.Id == u3.Id, u3);
            userCollec.ReplaceOne<User>(u => u.Id == u4.Id, u4);
            userCollec.ReplaceOne<User>(u => u.Id == u5.Id, u5);


            //Adding circles
            Circle c1 = new Circle(){Name = "c1", OwnerId = u1.Id};
            Circle c2 = new Circle() { Name = "c2", OwnerId = u2.Id};

            circleCollec.InsertOne(c1);
            circleCollec.InsertOne(c2);

            //adding circle walls
            Wall wc1 = new Wall() {owner = c1.Name, ownerID = c1.Id, type = "Circle" };
            Wall wc2 = new Wall() {owner = c2.Name, ownerID = c2.Id, type = "Circle" };

            wallCollec.InsertOne(wc1);
            wallCollec.InsertOne(wc2);

            

        
            //adding walls to circles
            c1.WallId = wc1.ID;
            c2.WallId = wc2.ID;
            circleCollec.ReplaceOne<Circle>(c => c.Id == c1.Id, c1);
            circleCollec.ReplaceOne<Circle>(c => c.Id == c2.Id, c2);


            //adding users to circles
            wc1.Followers= new List<follower>();
            wc1.Followers.Add(new follower() { followerID = u1.Id, followerName = u1.Name });
            wc1.Followers.Add(new follower() { followerID = u2.Id, followerName = u2.Name });
            wc1.Followers.Add(new follower() { followerID = u3.Id, followerName = u3.Name });

            wc2.Followers = new List<follower>();
            wc2.Followers.Add(new follower() { followerID = u1.Id, followerName = u1.Name });
            wc2.Followers.Add(new follower() { followerID = u4.Id, followerName = u4.Name });
            wc2.Followers.Add(new follower() { followerID = u5.Id, followerName = u5.Name });

            wallCollec.ReplaceOne<Wall>(w => w.ID == wc1.ID, wc1);
            wallCollec.ReplaceOne<Wall>(w => w.ID == wc2.ID, wc2);


            //set users to follow users
            u1.Followers = new List<string>();
            u2.Followers = new List<string>();
            u3.Followers = new List<string>();
            u4.Followers = new List<string>();
            u5.Followers = new List<string>();

            u1.FollowingId = new List<string>();
            u2.FollowingId = new List<string>();
            u3.FollowingId = new List<string>();
            u4.FollowingId = new List<string>();
            u5.FollowingId = new List<string>();


            u1.Followers.Add(u2.Id);
            u1.Followers.Add(u3.Id);
            u1.Followers.Add(u4.Id);
            u2.FollowingId.Add(u1.Id);
            u3.FollowingId.Add(u1.Id);
            u4.FollowingId.Add(u1.Id);

            u2.Followers.Add(u1.Id);
            u2.Followers.Add(u3.Id);
            u1.FollowingId.Add(u2.Id);
            u3.FollowingId.Add(u2.Id);

            u3.Followers.Add(u2.Id);
            u3.Followers.Add(u5.Id);
            u2.FollowingId.Add(u3.Id);
            u5.FollowingId.Add(u3.Id);

            u4.Followers.Add(u3.Id);
            u4.Followers.Add(u5.Id);
            u3.FollowingId.Add(u4.Id);
            u5.FollowingId.Add(u4.Id);

            u5.Followers.Add(u1.Id);
            u5.Followers.Add(u2.Id);
            u1.FollowingId.Add(u5.Id);
            u2.FollowingId.Add(u5.Id);

            userCollec.ReplaceOne<User>(u => u.Id == u1.Id, u1);
            userCollec.ReplaceOne<User>(u => u.Id == u2.Id, u2);
            userCollec.ReplaceOne<User>(u => u.Id == u3.Id, u3);
            userCollec.ReplaceOne<User>(u => u.Id == u4.Id, u4);
            userCollec.ReplaceOne<User>(u => u.Id == u5.Id, u5);


            //Adding posts
            Post p1 = new Post(){CreationTime = DateTime.Now, OwnerId = u1.Id, OwnerName = u1.Name, Text = "Dette er en post 1", WallId = u1.Wall};
            Post p2 = new Post() { CreationTime = DateTime.Now, OwnerId = u1.Id, OwnerName = u1.Name, Text = "Dette er en post 2", WallId = u1.Wall };

            Post p3 = new Post() { CreationTime = DateTime.Now, OwnerId = u2.Id, OwnerName = u2.Name, Text = "Dette er en post 3", WallId = u2.Wall };
            Post p4 = new Post() { CreationTime = DateTime.Now, OwnerId = u2.Id, OwnerName = u2.Name, Text = "Dette er en post 4", WallId = u2.Wall };

            Post p5 = new Post() { CreationTime = DateTime.Now, OwnerId = u3.Id, OwnerName = u3.Name, Text = "Dette er en post 5", WallId = u3.Wall };
            Post p6 = new Post() { CreationTime = DateTime.Now, OwnerId = u3.Id, OwnerName = u3.Name, Text = "Dette er en post 6", WallId = u3.Wall };

            Post p7 = new Post() { CreationTime = DateTime.Now, OwnerId = u4.Id, OwnerName = u4.Name, Text = "Dette er en post 7", WallId = u4.Wall };
            Post p8 = new Post() { CreationTime = DateTime.Now, OwnerId = u4.Id, OwnerName = u4.Name, Text = "Dette er en post 8", WallId = u4.Wall };

            Post p9 = new Post() { CreationTime = DateTime.Now, OwnerId = u5.Id, OwnerName = u5.Name, Text = "Dette er en post 9", WallId = u5.Wall };
            Post p10 = new Post() { CreationTime = DateTime.Now, OwnerId = u5.Id, OwnerName = u5.Name, Text = "Dette er en post 10", WallId = u5.Wall };

            Post p11 = new Post() { CreationTime = DateTime.Now, OwnerId = u1.Id, OwnerName = u1.Name, Text = "Dette er en post 11", WallId = c1.WallId };
            Post p12 = new Post() { CreationTime = DateTime.Now, OwnerId = u1.Id, OwnerName = u1.Name, Text = "Dette er en post 12", WallId = c1.WallId };

            Post p13 = new Post() { CreationTime = DateTime.Now, OwnerId = u2.Id, OwnerName = u2.Name, Text = "Dette er en post 13", WallId = c2.WallId };
            Post p14 = new Post() { CreationTime = DateTime.Now, OwnerId = u2.Id, OwnerName = u2.Name, Text = "Dette er en post 14", WallId = c2.WallId };

            postCollec.InsertOne(p1);
            postCollec.InsertOne(p2);
            postCollec.InsertOne(p3);
            postCollec.InsertOne(p4);
            postCollec.InsertOne(p5);
            postCollec.InsertOne(p6);
            postCollec.InsertOne(p7);
            postCollec.InsertOne(p8);
            postCollec.InsertOne(p9);
            postCollec.InsertOne(p10);
            postCollec.InsertOne(p11);
            postCollec.InsertOne(p12);
            postCollec.InsertOne(p13);
            postCollec.InsertOne(p14);

            //Adding posts to wall
            wu1.postIDs = new List<string>();
            wu2.postIDs = new List<string>();
            wu3.postIDs = new List<string>();
            wu4.postIDs = new List<string>();
            wu5.postIDs = new List<string>();
            wc1.postIDs = new List<string>();
            wc2.postIDs = new List<string>();

            wu1.postIDs.Add(p1.Id);
            wu1.postIDs.Add(p2.Id);

            wu2.postIDs.Add(p3.Id);
            wu2.postIDs.Add(p4.Id);

            wu3.postIDs.Add(p5.Id);
            wu3.postIDs.Add(p6.Id);

            wu4.postIDs.Add(p7.Id);
            wu4.postIDs.Add(p8.Id);

            wu5.postIDs.Add(p9.Id);
            wu5.postIDs.Add(p10.Id);

            wc1.postIDs.Add(p11.Id);
            wc1.postIDs.Add(p12.Id);

            wc2.postIDs.Add(p13.Id);
            wc2.postIDs.Add(p14.Id);

            wallCollec.ReplaceOne<Wall>(w => w.ID == wu1.ID, wu1);
            wallCollec.ReplaceOne<Wall>(w => w.ID == wu2.ID, wu2);
            wallCollec.ReplaceOne<Wall>(w => w.ID == wu3.ID, wu3);
            wallCollec.ReplaceOne<Wall>(w => w.ID == wu4.ID, wu4);
            wallCollec.ReplaceOne<Wall>(w => w.ID == wu5.ID, wu5);
            wallCollec.ReplaceOne<Wall>(w => w.ID == wc1.ID, wc1);
            wallCollec.ReplaceOne<Wall>(w => w.ID == wc2.ID, wc2);


            Comment co1 = new Comment(){ OwnerId = u1.Id, OwnerName = u1.Name, Post = p1.Id, Text = "Lol"};
            Comment co2 = new Comment() { OwnerId = u1.Id, OwnerName = u1.Name, Post = p1.Id, Text = "Awesome" };
            Comment co3 = new Comment() { OwnerId = u1.Id, OwnerName = u1.Name, Post = p2.Id, Text = "Lol" };
            Comment co4 = new Comment() { OwnerId = u1.Id, OwnerName = u1.Name, Post = p3.Id, Text = "Beautiful" };
            Comment co5 = new Comment() { OwnerId = u1.Id, OwnerName = u1.Name, Post = p4.Id, Text = "Lol" };

            commentCollec.InsertOne(co1);
            commentCollec.InsertOne(co2);
            commentCollec.InsertOne(co3);
            commentCollec.InsertOne(co4);
            commentCollec.InsertOne(co5);

            p1.Comments = new List<Comment>();
            p2.Comments = new List<Comment>();
            p3.Comments = new List<Comment>();
            p4.Comments = new List<Comment>();

            p1.Comments.Add(co1);
            p1.Comments.Add(co2);
            p2.Comments.Add(co3);
            p3.Comments.Add(co4);
            p4.Comments.Add(co5);

            postCollec.ReplaceOne<Post>(p => p.Id == p1.Id, p1);
            postCollec.ReplaceOne<Post>(p => p.Id == p2.Id, p2);
            postCollec.ReplaceOne<Post>(p => p.Id == p3.Id, p3);
            postCollec.ReplaceOne<Post>(p => p.Id == p4.Id, p4);

        }

    }
}
