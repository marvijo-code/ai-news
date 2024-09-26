import React from 'react';

interface NewsItemProps {
  title: string;
  description: string;
  url: string;
}

const NewsItem: React.FC<NewsItemProps> = ({ title, description, url }) => {
  return (
    <div className="news-item">
      <h2>{title}</h2>
      <p>{description}</p>
      <a href={url} target="_blank" rel="noopener noreferrer">Read more</a>
    </div>
  );
};

export default NewsItem;
