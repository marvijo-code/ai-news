import React from 'react';
import NewsList from './components/NewsList';
import './App.css';

const App: React.FC = () => {
  return (
    <div className="App">
      <header className="App-header">
        <h1>AI News API</h1>
      </header>
      <main>
        <NewsList />
      </main>
    </div>
  );
};

export default App;
