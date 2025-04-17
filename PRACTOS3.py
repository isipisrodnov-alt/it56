import json

class Anime:
    def __init__(self, title, genre, age_rating):
        self.title = title
        self.genre = genre
        self.age_rating = age_rating

    def __str__(self):
        return f"{self.title} (Жанр: {self.genre}, Возрастное ограничение: {self.age_rating})"

class AnimeCollection:
    def __init__(self):
        self.data = []

    def add_item(self, anime):
        self.data.append(anime)
        print(f"Аниме '{anime.title}' добавлено.")

    def remove_item(self, title):
        self.data = [anime for anime in self.data if anime.title != title]
        print(f"Аниме '{title}' удалено.")

    def search_item(self, keyword):
        results = [anime for anime in self.data if keyword.lower() in anime.title.lower()]
        return results

    def sort_data(self, by='title'):
        if by == 'title':
            return sorted(self.data, key=lambda x: x.title)
        elif by == 'length':
            return sorted(self.data, key=lambda x: len(x.title))
        else:
            return self.data

    def filter_data(self, genre):
        return [anime for anime in self.data if anime.genre.lower() == genre.lower()]

    def export_data(self, filename):
        with open(filename, 'w', encoding='utf-8') as f:
            json.dump([anime.__dict__ for anime in self.data], f, ensure_ascii=False, indent=4)
        print(f"Данные экспортированы в файл {filename}.")

    def import_data(self, filename):
        with open(filename, 'r', encoding='utf-8') as f:
            anime_list = json.load(f)
            for anime_data in anime_list:
                anime = Anime(**anime_data)
                self.add_item(anime)
        print(f"Данные импортированы из файла {filename}.")

def display_menu():
    print("\nВыберите действие:")
    print("1. Добавить аниме")
    print("2. Удалить аниме")
    print("3. Поиск аниме")
    print("4. Сортировка")
    print("5. Фильтрация")
    print("6. Экспорт данных")
    print("7. Импорт данных")
    print("8. Выход")

def main():
    collection = AnimeCollection()
    while True:
        display_menu()
        choice = input("Ваш выбор: ")

        if choice == '1':
            title = input("Введите аниме для добавления: ")
            genre = input("Введите жанр аниме: ")
            age_rating = input("Введите возрастное ограничение аниме: ")
            anime = Anime(title.strip(), genre.strip(), age_rating.strip())
            collection.add_item(anime)
        elif choice == '2':
            title = input("Введите аниме для удаления: ")
            collection.remove_item(title.strip())
        elif choice == '3':
            keyword = input("Введите ключевое слово для поиска: ")
            results = collection.search_item(keyword)
            if results:
                print("Найденные аниме:")
                for anime in results:
                    print(anime)
            else:
                print("Аниме не найдены.")
        elif choice == '4':
            print("\nВыберите способ сортировки:")
            print("1. По алфавиту")
            print("2. По количеству символов")
            sort_choice = input("Ваш выбор: ")
            if sort_choice == '1':
                sorted_data = collection.sort_data('title')
                print("Отсортированные аниме по алфавиту:")
            elif sort_choice == '2':
                sorted_data = collection.sort_data('length')
                print("Отсортированные аниме по количеству символов:")
            else:
                print("Некорректный ввод. Сортировка не выполнена.")
                continue

            for anime in sorted_data:print(anime)
        elif choice == '5':
            genre_filter = input("Введите жанр для фильтрации: ")
            filtered = collection.filter_data(genre_filter)
            print("Отфильтрованные данные:")
            for anime in filtered:
                print(anime)
        elif choice == '6':
            filename = input("Введите имя файла для экспорта: ")
            collection.export_data(filename)
        elif choice == '7':
            filename = input("Введите имя файла для импорта: ")
            collection.import_data(filename)
        elif choice == '8':
            print("Выход из программы.")
            break
        else:
            print("Некорректный ввод. Пожалуйста, выберите действие из меню.")

if __name__ == "__main__":
    main()