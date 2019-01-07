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
        private bool _checkboxRandomFusions;
        private bool _checkboxRandomStarchips;
        private int _textboxMinAttack = 1000;
        private int _textboxMaxAttack = 3000;
        private int _textboxMinDefense = 1000;
        private int _textboxMaxDefense = 3000;
        private int _textboxMinCost = 0;
        private int _textboxMaxCost = 999999;
        private bool _checkboxIsoSeed = true;
        private bool _checkboxIsoDate;
        private bool _checkboxIsoOptions;
        private bool _checkboxSpoilerFiles = true;
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
                onIsoCheckbox();
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
                onIsoCheckbox();
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
                onIsoCheckbox();
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
                onIsoCheckbox();
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
                onIsoCheckbox();
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
                onIsoCheckbox();
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
                onIsoCheckbox();
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
                onIsoCheckbox();
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
                onIsoCheckbox();
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
                onIsoCheckbox();
            }
        }

        public bool CheckboxRandomStarchips
        {
            get => _checkboxRandomStarchips;
            set
            {
                _checkboxRandomStarchips = value;
                Static.RandomStarchips = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxRandomStarchips"));
                onIsoCheckbox();
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

        public int TextboxMinCost
        {
            get => _textboxMinCost;
            set
            {
                _textboxMinCost = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMinCost"));
            }
        }

        public int TextboxMaxCost
        {
            get => _textboxMaxCost;
            set
            {
                _textboxMaxCost = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextboxMaxCost"));
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

        public bool CheckboxSpoilerFiles
        {
            get => _checkboxSpoilerFiles;
            set
            {
                _checkboxSpoilerFiles = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CheckboxSpoilerFiles"));
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

            if (CheckboxIsoSeed) LabelIsoExample += $"[{_textboxSeed}]";
            if (CheckboxIsoOptions)
            {
                var options_str = "";
                if (Static.RandomAtkdef) options_str += "[ATKDEF]";
                if (Static.RandomAttributes) options_str += "[Attributes]";
                if (Static.RandomCardDrops) options_str += "[Drops]";
                if (Static.RandomDecks) options_str += "[Decks]";
                if (Static.RandomEquips) options_str += "[Equips]";
                if (Static.RandomFusions) options_str += "[Fusions]";
                if (Static.RandomGuardianStars) options_str += "[Guardian_Stars]";
                if (Static.RandomTypes) options_str += "[Types]";
                if (Static.RandomStarchips) options_str += "[Starchips]";
                LabelIsoExample += options_str;
            }
            if (CheckboxIsoDate) LabelIsoExample += $"[{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}]";

            Static.RandomizerFileName = LabelIsoExample;

            LabelIsoExample += ".bin";
        }

    }
}
