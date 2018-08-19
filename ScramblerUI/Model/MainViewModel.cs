using System;
using System.ComponentModel;
using FMLib.Utility;

namespace FMScrambler.Model
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string _labelPath = "Nothing selected.";
        private string _textboxSeed;
        private bool _checkboxRandomAttributes;
        private bool _checkboxRandomTypes;
        private bool _checkboxRandomGuardianStars;
        private bool _checkboxGlitchGuardianStars;
        private bool _checkboxRandomCardDrops;
        private bool _checkboxRandomDecks;
        private bool _checkboxAttackDefenseRandomizing;
        private bool _checkboxGlitchCards;
        private bool _checkboxRandomEquips;
        private bool _checkboxRandomFusions = true;
        private int _textboxMinAttack = 1000;
        private int _textboxMaxAttack = 3000;
        private int _textboxMinDefense = 1000;
        private int _textboxMaxDefense = 3000;
        private bool _checkboxIsoSeed = true;
        private bool _checkboxIsoDate;
        private bool _checkboxIsoOptions;
        private string _labelIsoExample = "fmscrambler[12345678].bin";

        public string LabelPath
        {
            get => _labelPath;
            set
            {
                _labelPath = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LabelPath"));
            }
        }

        public string TextboxSeed
        {
            get => _textboxSeed;
            set
            {
                _textboxSeed = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxSeed"));
            }
        }

        public bool CheckboxRandomAttributes
        {
            get => _checkboxRandomAttributes;
            set
            {
                _checkboxRandomAttributes = value;
                Static.RandomAttributes = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomAttributes"));
            }
        }

        public bool CheckboxRandomTypes
        {
            get => _checkboxRandomTypes;
            set
            {
                _checkboxRandomTypes = value;
                Static.RandomTypes = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomTypes"));
            }
        }

        public bool CheckboxRandomGuardianStars
        {
            get => _checkboxRandomGuardianStars;
            set
            {
                _checkboxRandomGuardianStars = value;
                Static.RandomGuardianStars = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomGuardianStars"));
            }
        }

        public bool CheckboxGlitchGuardianStars
        {
            get => _checkboxGlitchGuardianStars;
            set
            {
                _checkboxGlitchGuardianStars = value;
                Static.GlitchGuardianStars = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxGlitchGuardianStars"));
            }
        }

        public bool CheckboxRandomCardDrops
        {
            get => _checkboxRandomCardDrops;
            set
            {
                _checkboxRandomCardDrops = value;
                Static.RandomCardDrops = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomCardDrops"));
            }
        }

        public bool CheckboxRandomDecks
        {
            get => _checkboxRandomDecks;
            set
            {
                _checkboxRandomDecks = value;
                Static.RandomDecks = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomDecks"));
            }
        }

        public bool CheckboxAttackDefenseRandomizing
        {
            get => _checkboxAttackDefenseRandomizing;
            set
            {
                _checkboxAttackDefenseRandomizing = value;
                Static.RandomAtkdef = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxAttackDefenseRandomizing"));
            }
        }

        public bool CheckboxGlitchCards
        {
            get => _checkboxGlitchCards;
            set
            {
                _checkboxGlitchCards = value;
                Static.GlitchFusions = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxGlitchCards"));
            }
        }

        public bool CheckboxRandomEquips
        {
            get => _checkboxRandomEquips;
            set
            {
                _checkboxRandomEquips = value;
                Static.RandomEquips = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomEquips"));
            }
        }

        public bool CheckboxRandomFusions
        {
            get => _checkboxRandomFusions;
            set
            {
                _checkboxRandomFusions = value;
                Static.RandomFusions = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomFusions"));
            }
        }

        public int TextboxMinAttack
        {
            get => _textboxMinAttack;
            set
            {
                _textboxMinAttack = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMinAttack"));
            }
        }

        public int TextboxMaxAttack
        {
            get => _textboxMaxAttack;
            set
            {
                _textboxMaxAttack = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMaxAttack"));
            }
        }

        public int TextboxMinDefense
        {
            get => _textboxMinDefense;
            set
            {
                _textboxMinDefense = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMinDefense"));
            }
        }

        public int TextboxMaxDefense
        {
            get => _textboxMaxDefense;
            set
            {
                _textboxMaxDefense = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMaxDefense"));
            }
        }

        public string VersionMeta => $"{Meta.MajorVersion}.{Meta.MinorVersion}.{Meta.PatchVersion} {Meta.VersionInfo}";

        public bool CheckboxIsoSeed
        {
            get => _checkboxIsoSeed;
            set
            {
                _checkboxIsoSeed = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxIsoSeed"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxIsoDate
        {
            get => _checkboxIsoDate;
            set
            {
                _checkboxIsoDate = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxIsoDate"));
                onIsoCheckbox();
            }
        }

        public bool CheckboxIsoOptions
        {
            get => _checkboxIsoOptions;
            set
            {
                _checkboxIsoOptions = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxIsoOptions"));
                onIsoCheckbox();
            }
        }

        public string LabelIsoExample
        {
            get => _labelIsoExample;
            set
            {
                _labelIsoExample = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LabelIsoExample"));
            }
        }

        private void onIsoCheckbox()
        {
            if (!CheckboxIsoSeed && !CheckboxIsoOptions && !CheckboxIsoDate) CheckboxIsoSeed = true; 

            LabelIsoExample = "fmscrambler";

            if (CheckboxIsoSeed) LabelIsoExample += "[12345678]";
            if (CheckboxIsoOptions) LabelIsoExample += "[Fusions][ATKDEF][Drops]";
            if (CheckboxIsoDate) LabelIsoExample += "[2018-01-01]";

            LabelIsoExample += ".bin";
        }

    }
}
