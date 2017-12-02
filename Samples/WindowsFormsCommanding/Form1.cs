using System;
using System.Windows.Forms;
using WindowsFormsCommanding.Commands;
using WindowsFormsCommanding.Handlers;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace WindowsFormsCommanding
{
    public partial class Form1 : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandDispatcher _dispatcher;

        public Form1()
        {
            InitializeComponent();
            IServiceCollection serviceCollection = new ServiceCollection();
            ICommandingDependencyResolver resolver = new CommandingDependencyResolver((type, instance) => serviceCollection.AddSingleton(type, instance),
                (type, impl) => serviceCollection.AddTransient(type, impl),
                type => _serviceProvider.GetService(type));
            ICommandRegistry registry = resolver.UseCommanding();
            registry.Register<GetTextCommandHandler>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _dispatcher = _serviceProvider.GetService<ICommandDispatcher>();
        }

        private void _runCommandButton_Click(object sender, EventArgs e)
        {
            string result = _dispatcher.DispatchAsync(new GetTextCommand()).Result;
            _textbox.Text = result;
        }
    }
}
