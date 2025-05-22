namespace OrderService.Api.Extensions;

public static class ServiceCollectionsExtensions
{
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Orders Api",
                Version = "v1"
            });

            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter valid tocken",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return builder;
    }   
    
    public static WebApplicationBuilder AddData(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<OrderDbContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        return builder;
    }
    
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICartsService, CartsService>();
        builder.Services.AddScoped<IMerchantsService, MerchantsService>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        return builder;
    }
    
    public static WebApplicationBuilder AddBearerAuthentication(this WebApplicationBuilder builder) 
        {
            //для авторизации  используем Jwt bearer схему
            builder.Services.AddAuthentication(x =>
            {               
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                //добовляем jwt bearer схему
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.UseSecurityTokenValidators = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                        builder.Configuration["Authentication:TokenPrivateKey"]!)),
                    ValidIssuer = "test",
                    ValidAudience = "test",
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
            //подключаем авторизацию
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole(RoleConsts.Admin));
                options.AddPolicy("Merchant", policy => policy.RequireRole(RoleConsts.Merchant));
                options.AddPolicy("User", policy => policy.RequireRole(RoleConsts.User));
            });
            builder.Services.AddTransient<IAuthService, AuthService>();
            builder.Services.AddDefaultIdentity<UserEntity>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
           })
          .AddEntityFrameworkStores<OrderDbContext>()
          .AddUserManager<UserManager<UserEntity>>()
          .AddUserStore<UserStore<UserEntity, IdentityRoleEntity, OrderDbContext, long>>();
        
            return builder;
        }
    
    public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Authentication"));
        builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));

        return builder;
    }
    
    public static WebApplicationBuilder AddBackgroundService(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<CreateOrderConsumer>();
        
        return builder;
    }

    public static WebApplicationBuilder AddElastic(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, config) =>
        {
            config
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["Elasticsearch:Url"]))
                {
                    IndexFormat = $"{context.Configuration["ApplicationName"]}-logs-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true,
                    NumberOfShards = 2,
                    NumberOfReplicas = 1,
                    ModifyConnectionSettings = x => x
                        .BasicAuthentication(
                            context.Configuration["Elasticsearch:Username"],
                            context.Configuration["Elasticsearch:Password"])
                        .ServerCertificateValidationCallback((o, cert, chain, errors) => true)
                })
                .ReadFrom.Configuration(context.Configuration);
        });
        return builder;
    }
}