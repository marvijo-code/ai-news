import React from 'react';
import NewsItem from './NewsItem';

function NewsList({ news }) {
  return (
    <div className="news-list">
      {news.map((item, index) => (
        <NewsItem key={index} item={item} />
      ))}
    </div>
  );
}

export default NewsList;
