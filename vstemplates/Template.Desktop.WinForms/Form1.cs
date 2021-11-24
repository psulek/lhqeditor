using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Template.Desktop.WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Setup();
        }

        private void Setup()
        {
            var activeCulture = StringsContext.Instance.Culture.Name;
            int activeIdx = -1;

            for (int i = 0; i < StringsContext.Instance.AvailableCultures.Length; i++)
            {
                var culture = StringsContext.Instance.AvailableCultures[i];
                cmbLanguages.Items.Add(culture);

                if (activeCulture == culture)
                {
                    activeIdx = i;
                }
            }

            cmbLanguages.SelectedIndex = activeIdx;

            BindControls();
        }

        private void BindControls()
        {
            var binding1 = new Binding(nameof(Label.Text), StringsBindingProxy.Instance, StringsKeys.Messages.Welcome);
            lbWelcomeMessage.DataBindings.Add(binding1);


            var binding2 = new Binding(nameof(Label.Text), StringsBindingProxy.Instance,
                StringsKeys.Messages.Title);
            binding2.Format += delegate(object sender, ConvertEventArgs args)
            {
                args.Value = string.Format((string) args.Value, Environment.UserName, "Localization HQ Editor",
                    DateTime.UtcNow);
            };
            lbTitle.DataBindings.Add(binding2);
        }

        private void cmbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            string culture = cmbLanguages.SelectedItem as string;

            if (StringsContext.Instance.Culture.Name != culture)
            {
                StringsContext.Instance.Culture = new CultureInfo(culture);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var message = Strings.Messages.TitleWithParams(Environment.UserName, 
              "Localization HQ Editor", DateTime.UtcNow);

            MessageBox.Show(message);
        }

        
    }
}
