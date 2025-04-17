import threading
import time
import os
import json
from datetime import datetime

# Файл для хранения данных пользователей
USER_DATA_FILE = 'users.json'
# Файл для хранения логов
LOG_FILE = 'app.log'
# Время лицензии в секундах (30 минут)
LICENSE_TIME = 30 * 60
# Пользовательская сессия
current_user = None
license_start_time = None

def log_action(message, user, error=False):
    timestamp = datetime.now().strftime('%d.%m.%Y %H:%M:%S')
    log_type = 'ERROR' if error else 'INFO'
    log_message = f"[{log_type}] [{timestamp}] [{user}] - {message}\n"
    with open(LOG_FILE, 'a') as log_file:
        log_file.write(log_message)

def save_data():
    global current_user
    while True:
        if current_user:
            with open(USER_DATA_FILE, 'w') as file:
                json.dump(current_user['shopping_list'], file)
        time.sleep(10)

def check_license():
    global license_start_time
    while True:
        if license_start_time and (time.time() - license_start_time) > LICENSE_TIME:
            print("Пробная лицензия программы завершена, чтобы продолжить работу приобретите лицензионный ключ!")
            break
        time.sleep(5)

def register(username, password):
    if os.path.exists(USER_DATA_FILE):
        with open(USER_DATA_FILE, 'r') as file:
            users = json.load(file)
    else:
        users = {}

    if username in users:
        print("Пользователь с таким именем уже существует.")
        return False

    users[username] = {'password': password, 'shopping_list': []}
    with open(USER_DATA_FILE, 'w') as file:
        json.dump(users, file)
    log_action("Пользователь зарегистрирован", username)
    return True

def login(username, password):
    global current_user, license_start_time
    with open(USER_DATA_FILE, 'r') as file:
        users = json.load(file)

    if username in users and users[username]['password'] == password:
        current_user = users[username]
        license_start_time = time.time()
        log_action("Пользователь вошел в систему", username)
        return True
    else:
        print("Неверный логин или пароль.")
        log_action("Попытка входа не удалась", username, error=True)
        return False

def add_item(item):
    if current_user:
        current_user['shopping_list'].append(item)
        log_action(f"Товар '{item}' добавлен в список покупок", current_user)
    else:
        print("Необходимо войти в систему.")

def remove_item(item):
    if current_user:
        try:
            current_user['shopping_list'].remove(item)
            log_action(f"Товар '{item}' удален из списка покупок", current_user)
        except ValueError:
            print("Товар не найден в списке.")
            log_action(f"Ошибка удаления товара '{item}': товар не найден", current_user, error=True)
    else:
        print("Необходимо войти в систему.")

def view_list():
    if current_user:
        print("Список покупок:", current_user['shopping_list'])
    else:
        print("Необходимо войти в систему.")

def main():
    threading.Thread(target=save_data, daemon=True).start()
    threading.Thread(target=check_license, daemon=True).start()

    while True:
        command = input("Введите команду (register, login, add, remove, view, exit): ").strip()
        if command == "register":
            username = input("Введите имя пользователя: ")
            password = input("Введите пароль: ")
            register(username, password)
        elif command == "login":
            username = input("Введите имя пользователя: ")
            password = input("Введите пароль: ")
            login(username, password)
        elif command == "add":
            item = input("Введите товар для добавления: ")
            add_item(item)
        elif command == "remove":
            item = input("Введите товар для удаления: ")
            remove_item(item)
        elif command == "view":
            view_list()
        elif command == "exit":
            break
        else:
            print("Неизвестная команда.")

if __name__ == "__main__":
    main()