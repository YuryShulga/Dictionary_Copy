using Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyDictionary
{
	public partial class MainForm : Form
	{
        private const string TextAppName = "Словари";
        private const string TextForEmpty = "Готов. Создайте или откройте словарь.";
		private const string TextForNewDictionary = "Создание нового словаря";
		private const string TextForOpenDictionary = "Открытие файла словаря";
		private const string TextForDictionaryWindow = "Обзор содержимого словаря";
		private const string TextForSaveAsDictionary = "Сохранение словаря";
		private const string TextForEditDictionaryProperties = "Изменение свойств словаря";
		private const string TextForAddWord = "Добавление нового слова в словарь";
		private const string TextForEditWord = "Изменение слова";
		private const string TextForFindWord = "Поиск слова в словаре";
		private const string TextForExportWord = "Експорт(сохранение) слова в файл";
        private MyDictionary Dictionary { get; set; }
		public MainForm()
		{
			InitializeComponent();
			this.Width = 600;
			this.Height = 600;
			this.Text = TextAppName;
			panelImage.Dock = DockStyle.Fill;
			panelDictionary.Height = this.Height - 65;
			panelDictionary.Width = this.Width - 12 - 10;
			panelDictionary.Location = new Point(1, 20);

			lbOriginalWords.Width = panelDictionary.Width / 2 - 11;
			lbDictionaryTranslations.Width = lbOriginalWords.Width;
			
			lbOriginalWords.Location = new Point(
				8,
				lbOriginalWords.Location.Y
				);
			lblWords.Location = new Point(
                lbOriginalWords.Location.X,
				lblWords.Location.Y
                );
			lbDictionaryTranslations.Location = new Point(
						 lbDictionaryTranslations.Width + 18,
						 lbDictionaryTranslations.Location.Y
						 );
			lblDictionaryTranslations.Location = new Point(
				lbDictionaryTranslations.Location.X,
				lblDictionaryTranslations.Location.Y
				);
			lbOriginalWords.Height = this.Height - lbOriginalWords.Location.Y - 90;
			lbDictionaryTranslations.Height = lbOriginalWords.Height;
			toolStrip1.Dock = DockStyle.Top;
			panelNew.Location = new Point(13, 50);
            panelDictionaryEdit.Location = new Point(13, 50);
            panelAddEditWord.Location = new Point(13, 50);
			panelFind.Location = new Point(13, 50);
			lblStatusBar.Text = TextForEmpty;
			int x = (this.Width - 13 - panelGreetings.Width) / 2;
			int y = (this.Height  - panelGreetings.Height) / 2 ;
            panelGreetings.Location=new Point(x, y);
			panelGreetings.Visible = true;
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
			NewDictionary();

		}

		/// <summary>
		/// отображение панели создания нового словаря
		/// </summary>
		private void NewDictionary()
		{
			
			if (!CheckChangedDictionary())
			{
				return;
			}
			NewDictionaryPanelShowHelper();
		}

		private void NewDictionaryPanelShowHelper()
		{
			panelGreetings.Visible = false;
			lblStatusBar.Text = TextForNewDictionary;
			panelDictionary.Visible = false;
			//-------
			newToolStripMenuItem.Enabled = false;
			newToolStripButton.Enabled = false;
			openToolStripButton.Enabled = false;
			openToolStripMenuItem.Enabled = false;
			saveToolStripMenuItem.Enabled = false;
			saveToolStripButton.Enabled = false;
			saveAsToolStripMenuItem.Enabled = false;
			fileRenameToolStripMenuItem.Enabled = false;
			//--------
			wordToolStripMenuItem.Visible = false;
			//------
			backToolStripMenuItem.Visible = true;
			backToolStripButton.Visible = true;
			//-----подготовка панели newDictionary
			tbDictionaryName.Text = "Какой-то словарь";
			tbOriginalLanguage.Text = "англо";
			tbTargetLanguage.Text = "русский";
			panelNew.Visible = true;
			tbDictionaryName.Focus();
		}

		private void btnCreate_Click(object sender, EventArgs e)
		{
			if (tbOriginalLanguage.Text == "" ||
				tbTargetLanguage.Text == "" ||
				tbDictionaryName.Text == "")
			{
				MessageBox.Show("Заполните все поля!", "Внимание");
				return;
			}
			Dictionary = new MyDictionary(tbDictionaryName.Text, tbOriginalLanguage.Text, tbTargetLanguage.Text);
			panelNew.Visible = false;
			backToolStripButton.Visible = false;
			backToolStripMenuItem.Visible = false;
            SetMainFraimText();
            //this.Text = TextAppName + "  - NoName";
			lbOriginalWords.Items.Clear();
			lbTranslations.Items.Clear();
			OpenDictionaryHelper();
			

		}
		
		private void btnCancel_Click(object sender, EventArgs e)
		{
			CancelNewDictionary();
			
		}

		/// <summary>
		/// Метод вызываемый при отмене создания нового словаря.
		/// </summary>
		private void CancelNewDictionary()
		{
			backToolStripMenuItem.Visible = false;
			backToolStripButton.Visible = false;
			if (Dictionary != null)
			{
				OpenDictionaryHelper();
				panelNew.Visible = false;
				return;
			}
			newToolStripMenuItem.Enabled = true;
			newToolStripButton.Enabled = true;
			openToolStripButton.Enabled = true;
			openToolStripMenuItem.Enabled = true;
			panelNew.Visible = false;

        }

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenDictionary();   
		}

		private void OpenDictionary()
		{
			if (!CheckChangedDictionary())
			{
				return;
			}
            panelGreetings.Visible = false;
            panelFind.Visible = false;
			panelDictionary.Enabled = true;
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
			lblStatusBar.Text = TextForOpenDictionary;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{

                panelDictionary.Visible = false;
                panelImage.Visible = true;
				panelImage.Refresh();

                bool isnull = false;
				if (Dictionary == null)
				{
					Dictionary = new MyDictionary("", "", "");
					isnull = true;
				}

                
                

                if (Dictionary.LoadFromFile(openFileDialog.FileName))
				{
					

                    lbOriginalWords.Items.Clear();
					lbOriginalWords.BeginUpdate();
					foreach (DictionaryWord item in Dictionary.Dictionary)
					{
						lbOriginalWords.Items.Add(item);
                    }
					lbOriginalWords.EndUpdate();
					OpenDictionaryHelper();
					lbOriginalWords_SelectedIndexChanged(null, null);
					SetMainFraimText();
					panelImage.Visible = false;
					MainForm_Resize(new Object(), new EventArgs());

                }
				else
				{
					MessageBox.Show("Файл словаря не найден", "Ошибка");
					if (isnull)
					{
						Dictionary = null;
					}
					panelImage.Visible = false;
					if (!isnull)
					{
                        panelDictionary.Visible = true;
                    }
                }

			}
			else //если отмена открытия
			{
				if (Dictionary != null)
				{
					lblStatusBar.Text = TextForDictionaryWindow;

				}
				else 
				{
					lblStatusBar.Text = TextForEmpty;
                }
			}

		}

		/// <summary>
		/// Устанавливает название главного окна в стиле "Словари - {имя текущего файла}"
		/// </summary>
		private void SetMainFraimText()
		{
			if (Dictionary.DictionaryFilePath == null)
			{
				this.Text = TextAppName + " - noname";
				return;

            }
            var fileName = Dictionary.DictionaryFilePath.Split('\\');
            this.Text = TextAppName + " - " + fileName[fileName.Length - 1];
        }

		/// <summary>
		/// подготавливает панель отображения словаря и все менюшки с кнопками
		/// (сам словарь не загружает в listBox)
		/// </summary>
		private void OpenDictionaryHelper()
		{
			panelGreetings.Visible = false;
			newToolStripMenuItem.Enabled = true;
			newToolStripButton.Visible = true;
			newToolStripButton.Enabled = true;
			openToolStripButton.Visible = true;
			openToolStripButton.Enabled = true;
			openToolStripMenuItem.Enabled = true;
			saveToolStripMenuItem.Enabled = true;
			saveToolStripButton.Enabled = true; 
			saveAsToolStripMenuItem.Enabled = true;
			fileRenameToolStripMenuItem.Enabled = true;
			//--------
			wordToolStripMenuItem.Visible = true;
			if (Dictionary.Dictionary.Count > 0)//если словарь не пустой
			{
				lbOriginalWords.SelectedIndex = 0;
				
				addToolStripButton.Enabled = true;
				addToolStripMenuItem.Enabled = true;
				editToolStripButton.Enabled = true;
				editToolStripMenuItem.Enabled = true;
				deleteToolStripButton.Enabled = true;
				deleteToolStripMenuItem.Enabled = true;
				findToolStripButton.Enabled = true;
				findWordToolStripMenuItem.Enabled = true;
				exportToolStripButton.Enabled = true;
				exportToolStripMenuItem.Enabled = true;

			}
			else //словарь пустой
			{
				addToolStripButton.Enabled = true;
				addToolStripMenuItem.Enabled = true;
				editToolStripButton.Enabled = false;
				editToolStripMenuItem.Enabled = false;
				deleteToolStripButton.Enabled = false;
				deleteToolStripMenuItem.Enabled = false;
				findToolStripButton.Enabled = true;
				findWordToolStripMenuItem.Enabled = true;
				exportToolStripButton.Enabled = false;
				exportToolStripMenuItem.Enabled = false;
			}
			lblDictionaryType.Text = Dictionary.OriginalLanguage + "-" + Dictionary.TargetLanguage;
			lblDictionaryName.Text = Dictionary.Name;
			lbDictionaryTranslations.Items.Clear();
			lblWords.Text = $"Слова({Dictionary.Dictionary.Count}шт.):";
			lblStatusBar.Text = TextForDictionaryWindow;
            panelDictionary.Visible = true;
			lbOriginalWords.Focus();
		}

		/// <summary>
		///  проверяет открыт и сохранен ли словарь, предлагает сохранить, либо отменить действие вызвавшее метод
		/// </summary>
		/// <returns>true - все хорошо, продолжаем. false - отменить выполнение метода вызвавшего данный</returns>
		/// <exception cref="DictionaryException"></exception>
		private bool CheckChangedDictionary()
		{

			if (Dictionary != null)//если есть открытый словарь
			{
				if (Dictionary.Changed)//если в словаре есть несохраненные изменения
				{
					DialogResult result = MessageBox.Show(
						$"Словарь \"{Dictionary.Name}\" не сохранен. Сохранить его перед закрытием?",
						"Внимание",
						MessageBoxButtons.YesNoCancel);
					//если сохранить
					if (result == DialogResult.No)//сохранять не нужно
					{
						return true;
					}
					//выбор кнопки отмена
					else if (result == DialogResult.Cancel)
					{
						return false;//(прерываем создание нового словаря  либо открытие словаря)
					}
					else//если выбор  - сохранить 
					{
						if (SaveHelper())
						{
							return true;//сохранить получилось
						}
						else
						{
							return false;//сохранить не получилось либо отмена
						}
					}

				}
			}
			return true;//можно продолжать выполнение команды(новый словарь либо открытие словаря)
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveHelper();
			
		}

		private bool SaveHelper()
		{
			//если словарь ни разу не сохранялся
			if (Dictionary.DictionaryFilePath == "" || Dictionary.DictionaryFilePath == null)
			{
				if (SaveAsHelper())
				{
					return true;
				}
				return false;
				
			}
			//попытка сохранить по уже известному пути
			if (Dictionary.SaveToFile())
			{
				return true;
			}
			else
			{
				MessageBox.Show("не смог сохранить словарь. Нет доступа к файлу.", "ошибка");
				return false;
			}
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAsHelper();
			

		}

		private bool SaveAsHelper()
		{
			SaveFileDialog save = new SaveFileDialog();
			save.InitialDirectory = Directory.GetCurrentDirectory();
			lblStatusBar.Text = TextForSaveAsDictionary;
			if (save.ShowDialog() == DialogResult.OK)
			{
				if (Dictionary.SaveAsToFile(save.FileName))
				{
					saveToolStripMenuItem.Enabled = true;
					SetMainFraimText();
					return true;
				}
				else
				{
					MessageBox.Show("файл не сохранен. нет доступа к файлу", "ошибка");
					return false;

				}
			}
			lblStatusBar.Text = TextForDictionaryWindow;
			return false;
		}


		private void addToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
			AddWord();
		}

		/// <summary>
		/// Подготавливает панель к добавлению слова и отображает ее ( а ткаже блокирует ненужные менюшки)
		/// </summary>
		private void AddWord()
		{
			//--подготовка основного интерфейса
			newToolStripMenuItem.Enabled = false;
			newToolStripButton.Enabled = false;
			openToolStripButton.Enabled = false;
			openToolStripMenuItem.Enabled = false;
			saveToolStripMenuItem.Enabled = false;
			saveToolStripButton.Enabled = false;
			saveAsToolStripMenuItem.Enabled = false;
			fileRenameToolStripMenuItem.Enabled = false;
			//--------
			wordToolStripMenuItem.Visible = false;
			backToolStripMenuItem.Visible = true;
			backToolStripButton.Visible = true;
			//--подготовка панели
			lblStatusBar.Text = TextForAddWord;
            btnAddTranslation.Text = "Добавить перевод";
			btnAddWord.Text = "Добавить слово в словарь";
			lblAddEditCaption.Text = "Добавление нового слова:";
			btnEditTranslation.Enabled = false;
			btnDeleteTranslation.Enabled = false;
			lblAddEditDictionaryType.Text = Dictionary.OriginalLanguage + "-" + Dictionary.TargetLanguage;
			tbNewEditOriginalWord.Text = "";
			tbNewEditTranslationWord.Text = "";
			lbTranslations.Items.Clear();

			panelDictionary.Visible = false;
			panelAddEditWord.Visible = true;
			tbNewEditOriginalWord.Focus();
		}

		private void btnCancelAddWord_Click(object sender, EventArgs e)
		{
			Back();
			
		}

		private void btnAddTranslation_Click(object sender, EventArgs e)
		{
			btnAddTranslation_ClickHelper(true);
		}
		/// <summary>
		/// обработчик нажатия кнопки "дибавить/изменить перевод слова из textBox в listBox"
		/// </summary>
		/// <param name="showMessages">true - показывать сообщения при ошибках</param>
		private void btnAddTranslation_ClickHelper(bool showMessages)
		{
			//если textbox с переводом пуст
			if (tbNewEditTranslationWord.Text == "")
			{
				if (showMessages)
				{
					MessageBox.Show("Перевод не введен", "Внимание");
				}
				return;
			}
            bool containsTranslation = false;
            foreach (WordTranslation trans in lbTranslations.Items)
            {
                if (trans.Name == tbNewEditTranslationWord.Text)
                {
                    containsTranslation = true;
                    break;
                }
            }
            if (btnAddTranslation.Text == "Изменить")
			{

				if (!containsTranslation)//если такого перевода нет в списке переводов
				{
					btnAddTranslation.Text = "Добавить перевод";
					lbTranslations.Items[lbTranslations.SelectedIndex] = new WordTranslation(tbNewEditTranslationWord.Text);
					tbNewEditTranslationWord.Text = "";
					btnEditTranslation.Enabled = true;
					btnDeleteTranslation.Enabled = true;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
					lbTranslations.Enabled = true;
				}
                else //если слово есть в списке переводов
                {
					if (tbNewEditTranslationWord.Text == ((WordTranslation)lbTranslations.Items[lbTranslations.SelectedIndex]).Name)
					{
						tbNewEditTranslationWord.Text = "";
						btnAddTranslation.Text = "Добавить перевод";
						btnEditTranslation.Enabled = true;
						btnDeleteTranslation.Enabled = true;
						lbTranslations.Enabled = true;
						return;
					}
					if (showMessages)
					{
						MessageBox.Show("Такой перевод уже есть у этого слова");
					}
					tbNewEditTranslationWord.Text = lbTranslations.Items[lbTranslations.SelectedIndex].ToString();
				}

				return;
			}
			//если это кнопка с названием "добавить"
			
            if (!containsTranslation)
            {
                lbTranslations.Items.Add(new WordTranslation(tbNewEditTranslationWord.Text));
                tbNewEditTranslationWord.Text = "";
            }
            else
            {
                if (showMessages)
                {
                    MessageBox.Show("Такой перевод уже есть у этого слова");
                }
            }
        }

		private void lbTranslations_Click(object sender, EventArgs e)
		{
			if (lbTranslations.SelectedIndex>-1)
			{
				if (btnAddTranslation.Text == "Изменить")
				{
					tbNewEditTranslationWord.Text = "";
					btnAddTranslation.Text = "Добавить";
					btnEditTranslation.Enabled = true;
				}
				btnEditTranslation.Enabled = true;
				btnDeleteTranslation.Enabled = true;
			}
			
		}

		private void btnDeleteTranslation_Click(object sender, EventArgs e)
		{
			lbTranslations.Items.RemoveAt(lbTranslations.SelectedIndex);
			if (lbTranslations.Items.Count == 0)
			{
				btnEditTranslation.Enabled = false;
				btnDeleteTranslation.Enabled = false;
			}
		}

		private void btnEditTranslation_Click(object sender, EventArgs e)
		{

			tbNewEditTranslationWord.Text = lbTranslations.Items[lbTranslations.SelectedIndex].ToString();
			btnAddTranslation.Text = "Изменить";
			btnEditTranslation.Enabled = false;
			btnDeleteTranslation.Enabled = false;
			lbTranslations.Enabled = false;
		}

		private void lbTranslations_DoubleClick(object sender, EventArgs e)
		{
			if (btnEditTranslation.Enabled)
			{
				btnEditTranslation_Click(sender, e);
			}
			
		}

		private void backToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Back();
		}

		private void Back()
		{
			if (panelFind.Visible)
			{
				int tempIndex = lbOriginalWords.SelectedIndex;
                backToolStripMenuItem.Visible = false;
				backToolStripButton.Visible = false;
                panelDictionary.Enabled = true;
                panelFind.Visible = false;
				OpenDictionaryHelper();
				lbOriginalWords.SelectedIndex= tempIndex;
				btnFind.Enabled = false;
				tbFindWord.Text = "";
				return;
			}
			if (panelDictionaryEdit.Visible)
			{
				CancelChangeDictionaryName();
				return;
			}
			if (panelNew.Visible)
			{
				CancelNewDictionary();
				return;
			}
			if (panelAddEditWord.Visible)
			{
                int tempSelectedIndex = lbOriginalWords.SelectedIndex;
                panelAddEditWord.Visible = false;
                backToolStripMenuItem.Visible = false;
				backToolStripButton.Visible = false;
                OpenDictionaryHelper();
				lbOriginalWords.SelectedIndex = tempSelectedIndex;
                lbOriginalWords_SelectedIndexChanged(null, null);
                return;
            }
		}

		private void btnAddWord_Click(object sender, EventArgs e)
		{
			
			btnAddTranslation_ClickHelper(false);
			//панель работает в режиме изменить слово
			if (btnAddWord.Text == "Подтвердить изменения")
			{
				int tempSelectedIndex = lbOriginalWords.SelectedIndex;
				List<WordTranslation> tempList = new List<WordTranslation>();
				foreach (WordTranslation item in lbTranslations.Items)
				{
					tempList.Add(item);
				}
				DictionaryWord word = DictionaryWord.NewWord(tbNewEditOriginalWord.Text, tempList);
				if (word == null)
				{
					MessageBox.Show("Заполните поля \"Слово\" и \"Перевод\"", "Внимание");
					return;
				}
				int result = Dictionary.EditWord(lbOriginalWords.SelectedIndex, word);
				
				if (result != 0)
				{
					switch (result)
					{
						case 1:
							{
								MessageBox.Show($"Исходного слова нет словаре", "Ошибка");
								return;
							}
						case 2:
							{
								MessageBox.Show($"Слово на которое меняем равно null", "Ошибка");
								return;
							}
						case 3:
							{
								MessageBox.Show($"Такое слово уже есть в словаре", "Ошибка изменения слова");
								tbNewEditOriginalWord.Text = lbOriginalWords.Text;
								return;
							}
					}

				}
				lbOriginalWords.Items[tempSelectedIndex] = Dictionary.Dictionary[tempSelectedIndex];
				//lbDictionaryTranslations.Items.Clear();
                panelAddEditWord.Visible = false;
				backToolStripMenuItem.Visible = false;
				backToolStripButton.Visible = false;
				OpenDictionaryHelper();
                lbOriginalWords.SelectedIndex = tempSelectedIndex;
                lbOriginalWords_SelectedIndexChanged(null, null);
                return;
			}
			//----------панель работает в режиме добавить слово
			List<WordTranslation> tempTranslationsList = new List<WordTranslation>();
			foreach (WordTranslation item in lbTranslations.Items)
			{
				tempTranslationsList.Add(item);
			}
			DictionaryWord newWord = DictionaryWord.NewWord(tbNewEditOriginalWord.Text, tempTranslationsList);
			if (newWord == null)
			{
				MessageBox.Show("Заполните поля \"Слово\" и \"Перевод\"", "Внимание");
				return;
			}
			if (!Dictionary.AddWord(newWord))
			{
				MessageBox.Show("Слово не добавлено. В словаре оно уже есть", "Ошибка добавления");
				return;
			}
            lbOriginalWords.Items.Add(Dictionary.Dictionary[Dictionary.Dictionary.Count - 1]);
            
            panelAddEditWord.Visible = false;
			backToolStripMenuItem.Visible = false;
			backToolStripButton.Visible = false;
			OpenDictionaryHelper();
            lbOriginalWords.SelectedIndex = lbOriginalWords.Items.Count - 1;
            lbOriginalWords_SelectedIndexChanged(sender, e);
        }   
		

		private void lbOriginalWords_SelectedIndexChanged(object sender, EventArgs e)
		{
			//если не выбрано какоето слово
			if (lbOriginalWords.SelectedIndex == -1)
			{
				editToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = false;
				exportToolStripMenuItem.Enabled = false;
				lbDictionaryTranslations.Items.Clear();
				return;

			}
			else//если выбрано слово
			{
				editToolStripMenuItem.Enabled = true;
				deleteToolStripMenuItem.Enabled = true;
				exportToolStripMenuItem.Enabled = true;

				


			}
			//вывод вывод вариантов перевода выбранного слова
			lbDictionaryTranslations.Items.Clear();
			foreach (WordTranslation item in ((DictionaryWord)lbOriginalWords.Items[lbOriginalWords.SelectedIndex]).Translation)
			{
				lbDictionaryTranslations.Items.Add(item);
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (Dictionary == null)
			{
				return;
			}
			if (!CheckChangedDictionary())
			{
				e.Cancel = true;
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void DeleteWord()
		{
			int selectedIndex = lbOriginalWords.SelectedIndex;
			if (!Dictionary.RemoveWord((DictionaryWord)lbOriginalWords.Items[lbOriginalWords.SelectedIndex]))
			{
				MessageBox.Show("Слово не получилось удалить. Оно отсутствует либо ссылка на него равна null");
				return;
			}
			lbOriginalWords.Items.RemoveAt(selectedIndex);
            OpenDictionaryHelper();
            if (lbOriginalWords.Items.Count > 0)
			{
				if (selectedIndex > 0)
				{
					selectedIndex--;
				}
				lbOriginalWords.SelectedIndex = selectedIndex;
			}
            lbOriginalWords_SelectedIndexChanged(null, null);
            Dictionary.Changed = true;

		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteWord();
		}

		private void EditWord()
		{
            //--подготовка основного интерфейса
            newToolStripMenuItem.Enabled = false;
            newToolStripButton.Enabled = false;
            openToolStripButton.Enabled = false;
            openToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            saveToolStripButton.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            fileRenameToolStripMenuItem.Enabled = false;
            //--------
            wordToolStripMenuItem.Visible = false;
            backToolStripMenuItem.Visible = true;
            backToolStripButton.Visible = true;
			//--подготовка панели


			lblStatusBar.Text = TextForEditWord;
            lblAddEditCaption.Text = "Изменение слова:";
            btnAddTranslation.Text = "Добавить перевод";
            btnAddWord.Text = "Подтвердить изменения";
            lblAddEditDictionaryType.Text = Dictionary.OriginalLanguage + "-" + Dictionary.TargetLanguage;
            tbNewEditOriginalWord.Text = lbOriginalWords.Items[lbOriginalWords.SelectedIndex].ToString();
            tbNewEditTranslationWord.Text = "";
            btnEditTranslation.Enabled = false;
            btnDeleteTranslation.Enabled = false;
            lbTranslations.Items.Clear();
            foreach (WordTranslation item in lbDictionaryTranslations.Items)
            {
                lbTranslations.Items.Add(item);
            }
            panelDictionary.Visible = false;
            panelAddEditWord.Visible = true;
            tbNewEditOriginalWord.Focus();
        }

		private void editToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditWord();

        }
	   

		private void lbOriginalWords_DoubleClick(object sender, EventArgs e)
		{
			if (lbOriginalWords.Items.Count > 0)
			{
				EditWord();

            }
			
		}

		private void findWordToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FindWord();

		}

		private void FindWord()
		{
            //--подготовка основного интерфейса
            newToolStripMenuItem.Enabled = false;
            newToolStripButton.Enabled = false;
            openToolStripButton.Enabled = false;
            openToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            saveToolStripButton.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            fileRenameToolStripMenuItem.Enabled = false;
            //--------
            wordToolStripMenuItem.Visible = false;
            backToolStripMenuItem.Visible = true;
            backToolStripButton.Visible = true;
			//--подготовка панели
			lblStatusBar.Text = TextForFindWord;
            panelDictionary.Enabled = false;
            panelFind.BringToFront();
            toolStrip1.BringToFront();
            panelFind.Visible = true;
			tbFindWord.Focus();
        }

		private void btnFindClose_Click(object sender, EventArgs e)
		{
			Back();

		}

		private void btnFind_Click(object sender, EventArgs e)
		{
			int findedIndex= Dictionary.FindWord(tbFindWord.Text);
			if (findedIndex == -1)
			{
				MessageBox.Show($"Слово \"{tbFindWord.Text}\" не найдено.");
				return;
			}
			lbOriginalWords.SelectedIndex = findedIndex;
			
			
		}

		private void tbFindWord_TextChanged(object sender, EventArgs e)
		{
			if (tbFindWord.Text != "")
			{
				btnFind.Enabled = true;
			}
			else
			{
				btnFind.Enabled = false;
			}
		}

		private void ExportWord()
		{
			SaveFileDialog save = new SaveFileDialog();
			save.InitialDirectory = Directory.GetCurrentDirectory();
			lblStatusBar.Text = TextForExportWord;

            if (save.ShowDialog() == DialogResult.OK)
			{
				if (!Dictionary.ExportWordToFile((DictionaryWord)lbOriginalWords.SelectedItem, save.FileName))
				{
					MessageBox.Show("файл не сохранен. нет доступа к файлу", "ошибка");
				}
			}
			lblStatusBar.Text = TextForDictionaryWindow;

        }

		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportWord();
		}

		private void fileRenameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Back();
			newToolStripMenuItem.Enabled = false;
			newToolStripButton.Enabled = false;
			openToolStripButton.Enabled = false;
			openToolStripMenuItem.Enabled = false;
			saveToolStripMenuItem.Enabled = false;
			saveToolStripButton.Enabled = false;
			saveAsToolStripMenuItem.Enabled = false;
			fileRenameToolStripMenuItem.Enabled = false;
			wordToolStripMenuItem.Visible = false;
			///---
			backToolStripMenuItem.Visible = true;
			backToolStripButton.Visible = true;
			tbDictionatyNameEdit.Text = Dictionary.Name;
			tbOriginalLanguageEdit.Text = Dictionary.OriginalLanguage;
			tbTargetLanguageEdit.Text = Dictionary.TargetLanguage;
			panelDictionary.Visible = false;
			panelDictionaryEdit.Visible = true;
			tbDictionatyNameEdit.Focus();
			lblStatusBar.Text = TextForEditDictionaryProperties;

		}

		public void CancelChangeDictionaryName()
		{
			OpenDictionaryHelper();
			backToolStripMenuItem.Visible = false;
			backToolStripButton.Visible = false;
			wordToolStripMenuItem.Visible = true;
		}

		private void btnCancelChangeDictionaryName_Click(object sender, EventArgs e)
		{
			CancelChangeDictionaryName();
			
		}

		private void btnSaveDictionaryName_Click(object sender, EventArgs e)
		{
			Dictionary.Name = tbDictionatyNameEdit.Text;
			Dictionary.OriginalLanguage=tbOriginalLanguageEdit.Text;
			Dictionary.TargetLanguage= tbTargetLanguageEdit.Text;
			Dictionary.Changed = true;
			Back();
		}

		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			NewDictionary();
		}

		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			OpenDictionary();
		}

		private void backToolStripButton_Click(object sender, EventArgs e)
		{
			Back();
		}

		private void addToolStripButton_Click(object sender, EventArgs e)
		{
			AddWord();
		}

		private void editToolStripButton_Click(object sender, EventArgs e)
		{
			EditWord();

        }

		private void deleteToolStripButton_Click(object sender, EventArgs e)
		{
			DeleteWord();
		}

		private void findToolStripButton_Click(object sender, EventArgs e)
		{
			FindWord();
		}

		private void exportToolStripButton_Click(object sender, EventArgs e)
		{
			ExportWord();
		}

		private void wordToolStripMenuItem_VisibleChanged(object sender, EventArgs e)
		{
			if (!wordToolStripMenuItem.Visible)
			{
				addToolStripButton.Visible = false;
				addToolStripButton.Enabled = false;
				//addToolStripMenuItem.Enabled = false;
				editToolStripButton.Visible = false;
				editToolStripButton.Enabled = false;
				//editToolStripMenuItem.Enabled = false;
				deleteToolStripButton.Visible = false;
				deleteToolStripButton.Enabled = false;
				//deleteToolStripMenuItem.Enabled = false;
				findToolStripButton.Visible = false;
				findToolStripButton.Enabled = false;
				//findWordToolStripMenuItem.Enabled = false;
				exportToolStripButton.Visible = false;
				exportToolStripButton.Enabled = false;
				//exportToolStripMenuItem.Enabled = false;
			}
			else
			{
				addToolStripButton.Visible = true;
				editToolStripButton.Visible = true;
				deleteToolStripButton.Visible = true;
				findToolStripButton.Visible = true;
				exportToolStripButton.Visible = true;
			}
		}

		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			SaveHelper();
		}

        private void tbFindWord_KeyPress(object sender, KeyPressEventArgs e)
        {
			if (e.KeyChar == 13) //кнопка Enter
			{
				btnFind_Click(null, null);

			}
			else CheckEscPressed(e);
        }

		/// <summary>
		/// проверяет нажата ли кнопка Esc, если да  - вызывает метод Back();
		/// </summary>
		/// <param name="e"></param>
		public void CheckEscPressed(KeyPressEventArgs e)
		{
            if (e.KeyChar == 27) 
            {
                Back();
            }
        }

        private void btnFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27) //кнопка Esc

            {
                Back();
            }
        }

        private void tbDictionaryName_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void tbOriginalLanguage_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void tbTargetLanguage_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void btnCreate_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void btnCancel_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void tbNewEditOriginalWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void tbNewEditTranslationWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void lbTranslations_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void btnAddTranslation_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void btnEditTranslation_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void btnDeleteTranslation_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void btnAddWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void btnCancelAddWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckEscPressed(e);
        }

        private void btnGreetNew_Click(object sender, EventArgs e)
        {
			panelGreetings.Visible = false;
            NewDictionary();
        }

        private void btnGreetOpen_Click(object sender, EventArgs e)
        {
            panelGreetings.Visible = false;
            OpenDictionary();
        }

        private void btnGreetExit_Click(object sender, EventArgs e)
        {
            panelGreetings.Visible = false;
            Close();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
			if (panelGreetings.Visible)
			{
                int x = (this.Width - 13 - panelGreetings.Width) / 2;
                int y = (this.Height - panelGreetings.Height) / 2;
                panelGreetings.Location = new Point(x, y);
            }
			if (panelDictionary.Visible)
			{
                panelDictionary.Height = this.Height - 65;
                panelDictionary.Width = this.Width - 12 - 10;
                lbOriginalWords.Width = panelDictionary.Width / 2 - 11;
                lbDictionaryTranslations.Width = lbOriginalWords.Width;
                lbOriginalWords.Location = new Point(
                    8,
                    lbOriginalWords.Location.Y
                    );
                lblWords.Location = new Point(
                    lbOriginalWords.Location.X,
                    lblWords.Location.Y
                    );
                lbDictionaryTranslations.Location = new Point(
                             lbDictionaryTranslations.Width + 18,
                             lbDictionaryTranslations.Location.Y
                             );
                lblDictionaryTranslations.Location = new Point(
                    lbDictionaryTranslations.Location.X,
                    lblDictionaryTranslations.Location.Y
                    );
                lbOriginalWords.Height = this.Height - lbOriginalWords.Location.Y - 90;
                lbDictionaryTranslations.Height = lbOriginalWords.Height;
            }
		}

        

        

    }
}
