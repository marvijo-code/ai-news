import React from 'react';

function NewsItem({ item }) {
  return (
    <div className="news-item">
      <h2>{item.title}</h2>
      <p>{item.description}</p>
      <a href={item.url} target="_blank" rel="noopener noreferrer">Read more</a>
    </div>
  );
}

export default NewsItem;
