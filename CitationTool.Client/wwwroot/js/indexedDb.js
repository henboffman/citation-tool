// IndexedDB wrapper for Citation Tool
const DB_NAME = 'CitationToolDB';
const DB_VERSION = 1;

let db = null;

// Quick search keyboard shortcut
window.setupQuickSearchShortcut = function(dotNetRef) {
    document.addEventListener('keydown', function(e) {
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('TriggerQuickSearch');
        }
    });
};

// Secure file download helper (avoids eval)
window.downloadFile = function(mimeType, base64Data, fileName) {
    const link = document.createElement('a');
    link.href = 'data:' + mimeType + ';base64,' + base64Data;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// Secure page reload helper (avoids eval)
window.reloadPage = function() {
    location.reload();
};

async function openDatabase() {
    if (db) return db;

    return new Promise((resolve, reject) => {
        const request = indexedDB.open(DB_NAME, DB_VERSION);

        request.onerror = () => reject(request.error);
        request.onsuccess = () => {
            db = request.result;
            resolve(db);
        };

        request.onupgradeneeded = (event) => {
            const database = event.target.result;

            // Citations store
            if (!database.objectStoreNames.contains('citations')) {
                const citationsStore = database.createObjectStore('citations', { keyPath: 'id' });
                citationsStore.createIndex('domainId', 'domainId', { unique: false });
                citationsStore.createIndex('type', 'type', { unique: false });
                citationsStore.createIndex('year', 'year', { unique: false });
                citationsStore.createIndex('dateAdded', 'dateAdded', { unique: false });
                citationsStore.createIndex('title', 'title', { unique: false });
            }

            // Domains store
            if (!database.objectStoreNames.contains('domains')) {
                const domainsStore = database.createObjectStore('domains', { keyPath: 'id' });
                domainsStore.createIndex('name', 'name', { unique: true });
            }

            // Settings store
            if (!database.objectStoreNames.contains('settings')) {
                database.createObjectStore('settings', { keyPath: 'key' });
            }
        };
    });
}

window.indexedDbInterop = {
    // Generic CRUD operations
    async add(storeName, item) {
        const database = await openDatabase();
        return new Promise((resolve, reject) => {
            const transaction = database.transaction(storeName, 'readwrite');
            const store = transaction.objectStore(storeName);
            const request = store.add(item);
            request.onsuccess = () => resolve(true);
            request.onerror = () => reject(request.error);
        });
    },

    async put(storeName, item) {
        const database = await openDatabase();
        return new Promise((resolve, reject) => {
            const transaction = database.transaction(storeName, 'readwrite');
            const store = transaction.objectStore(storeName);
            const request = store.put(item);
            request.onsuccess = () => resolve(true);
            request.onerror = () => reject(request.error);
        });
    },

    async delete(storeName, id) {
        const database = await openDatabase();
        return new Promise((resolve, reject) => {
            const transaction = database.transaction(storeName, 'readwrite');
            const store = transaction.objectStore(storeName);
            const request = store.delete(id);
            request.onsuccess = () => resolve(true);
            request.onerror = () => reject(request.error);
        });
    },

    async get(storeName, id) {
        const database = await openDatabase();
        return new Promise((resolve, reject) => {
            const transaction = database.transaction(storeName, 'readonly');
            const store = transaction.objectStore(storeName);
            const request = store.get(id);
            request.onsuccess = () => resolve(request.result || null);
            request.onerror = () => reject(request.error);
        });
    },

    async getAll(storeName) {
        const database = await openDatabase();
        return new Promise((resolve, reject) => {
            const transaction = database.transaction(storeName, 'readonly');
            const store = transaction.objectStore(storeName);
            const request = store.getAll();
            request.onsuccess = () => resolve(request.result || []);
            request.onerror = () => reject(request.error);
        });
    },

    async clear(storeName) {
        const database = await openDatabase();
        return new Promise((resolve, reject) => {
            const transaction = database.transaction(storeName, 'readwrite');
            const store = transaction.objectStore(storeName);
            const request = store.clear();
            request.onsuccess = () => resolve(true);
            request.onerror = () => reject(request.error);
        });
    },

    async count(storeName) {
        const database = await openDatabase();
        return new Promise((resolve, reject) => {
            const transaction = database.transaction(storeName, 'readonly');
            const store = transaction.objectStore(storeName);
            const request = store.count();
            request.onsuccess = () => resolve(request.result);
            request.onerror = () => reject(request.error);
        });
    },

    async bulkAdd(storeName, items) {
        const database = await openDatabase();
        return new Promise((resolve, reject) => {
            const transaction = database.transaction(storeName, 'readwrite');
            const store = transaction.objectStore(storeName);
            let addedCount = 0;

            items.forEach(item => {
                const request = store.put(item);
                request.onsuccess = () => addedCount++;
            });

            transaction.oncomplete = () => resolve(addedCount);
            transaction.onerror = () => reject(transaction.error);
        });
    },

    async getByIndex(storeName, indexName, value) {
        const database = await openDatabase();
        return new Promise((resolve, reject) => {
            const transaction = database.transaction(storeName, 'readonly');
            const store = transaction.objectStore(storeName);
            const index = store.index(indexName);
            const request = index.getAll(value);
            request.onsuccess = () => resolve(request.result || []);
            request.onerror = () => reject(request.error);
        });
    },

    async exportAllData() {
        const database = await openDatabase();
        const data = {
            citations: [],
            domains: [],
            settings: [],
            exportDate: new Date().toISOString(),
            version: DB_VERSION
        };

        const storeNames = ['citations', 'domains', 'settings'];

        for (const storeName of storeNames) {
            data[storeName] = await this.getAll(storeName);
        }

        return data;
    },

    async importAllData(data) {
        const database = await openDatabase();

        // Clear existing data
        await this.clear('citations');
        await this.clear('domains');

        // Import new data
        if (data.domains && data.domains.length > 0) {
            await this.bulkAdd('domains', data.domains);
        }

        if (data.citations && data.citations.length > 0) {
            await this.bulkAdd('citations', data.citations);
        }

        return true;
    },

    async deleteDatabase() {
        if (db) {
            db.close();
            db = null;
        }
        return new Promise((resolve, reject) => {
            const request = indexedDB.deleteDatabase(DB_NAME);
            request.onsuccess = () => resolve(true);
            request.onerror = () => reject(request.error);
        });
    }
};
