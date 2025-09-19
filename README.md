[![.NET](https://img.shields.io/badge/.NET_8.0-purple?logo=.net)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![EF Core](https://img.shields.io/badge/EF_Core-9.0-green)](https://learn.microsoft.com/ru-ru/ef/core/get-started/overview/install)
[![MS_SQL Server](https://img.shields.io/badge/MS_SQL_Server-2019+-yellow)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)


![ProjectChat Logo](assets/images/banner.png) 

# О проекте
**ProjectChat** — это ```клиент-серверное``` приложение, разработанное с целью углубления знаний в сетевом программировании и архитектурных паттернах. Начавшись как простой чат, проект превратился в полноценный мессенджер. Сейчас он завершен, так как основные цели достигнуты. Однако его совершенствование может продолжаться бесконечно. ✨

# 🌟 Ключевые моменты
- 🖥️ **Архитектура**: Клиент-серверная модель
- 👥 **Функционал**:
  - Авторизация пользователей
  - Личные и групповые чаты
  - Пользовательские аватарки
  - Настройки профиля
- 🔐 **Безопасность**: 
  - Шифрование паролей через BCrypt
  - Токен-аутентификация
- 🎨 **Кастомизация**:
  - Светлая/тёмная темы
  - Настройка размера шрифта сообщений


  ## 🛠️ Технологический стек
  | Компонент       | Технологии                     |
  |-----------------|--------------------------------|
  | **Клиент**      | WPF, MVVM, EventAggregator     |
  | **Сервер**      | TCP-сокеты, MS SQL Server, EF Core |
  | **Общие**       | .NET 8, JSON          |
  | **Безопасность**| BCrypt, токен-аутентификация   |

## 🗃️ Структура проекта
```
📦 ProjectChat
├── 📂ChatClient                # Клиентское приложение
│ ├── 📂Converters              # Конвертеры для WPF
│ ├── 📂CustomControls          # Кастомные элементы UI
│ ├── 📂Images                  # Ресурсы изображений
│ ├── 📂Services                # Сервисы (сеть, EventAggregator)
│ ├── 📂Styles                  # Стили и темы
│ ├── 📂View                    # Представления
│ └── 📂ViewModels              # ViewModels (MVVM)
│
├── 📂ChatShared                # Общая библиотека
│ ├── 📂DTO                     # Data Transfer Objects
│ └── 📂Events                  # Системные события
│
├── 📂ChatServer                # Серверная часть
  ├── 📂Data                    # Доступ к данным
  ├── 📂Handler                 # Обработчики запросов
  ├── 📂Migrations              # Миграции БД
  ├── 📂Models                  # Модели данных
  ├── 📂Services                # Бизнес-логика
  └── 📂Session                 # Управление сессиями
```
## 🖼️ Скриншоты
<p align="center">
    <img src="assets/images/Login.png" alt="ScreanShot2">
    <img src="assets/images/Main.jpg" alt="ScreanShot1">
    <img src="assets/images/Profile.png" alt="ScreanShot3">
    <img src="assets/images/LeftBoard.png" alt="ScreanShot3">
    <img src="assets/images/DarkTheme1.png" alt="ScreanShot3">
</p>

## 🐋 Развертывание сервера через Docker
### ⚙️ Подготовка среды
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows/Mac) или `docker.io` (Linux)
- [Docker Compose](https://docs.docker.com/compose/)
#### Установка на Linux
``` bash
sudo apt install docker.io
sudo apt install docker-compose
```

### Команды
``` bash
 git clone https://github.com/vZidv/ProjectChat.git
 cd ProjectChat
 docker-compose up --build
```

После запуска будут развернуты два **контейнера**:  
- MS SQL Server (база данных)
- Приложение сервера (ChatServer)

По умолчанию сервер будет доступен на порту ``8888``.

## 💡 Особенности реализации
1. **Сетевое взаимодействие**:
   - Обмен DTO через TCP в JSON-формате
   - EventAggregator для обработки событий
   - Асинхронные операции сокетов
   - Токен-аутентификация

2. **Клиент**:
   - MVVM с разделением View/ViewModel
   - Локальное хранение настроек в JSON
   - Кастомизация UI (темы, шрифты)
   - [ModernWpf](https://github.com/Kinnara/ModernWpf) для быстрых UI-набросков

3. **Сервер**:
   - EF Core для работы с SQL Server
   - BCrypt для шифрования паролей
   - Хранение аватаров в файловой системе
   - Управление сессиями пользователей

## 🚀 **Возможные улучшения**
  - Переход с Base64 на URL-формат
  - Добавить верификацию email через отправку кода
  - Возможность прикреплять документы (Файлообменник) 
  - Изменение/удаление сообщений
  - Desktop-оповещения о новых сообщениях
  - Индикация активности пользователей

## 📬 Контакты
✉️ **Email**: [diz.shulinus@yandex.ru](mailto:diz.shulinus@yandex.ru)  
📱 **Telegram**: [@mysarias](https://t.me/mysarias)

