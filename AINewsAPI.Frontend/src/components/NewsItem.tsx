import React from 'react';

interface NewsItemProps {
  title: string;
  description: string;
  url: string;
  formattedPublishedDate: string;
}

const decodeHtml = (html: string) => {
  const txt = document.createElement('textarea');
  txt.innerHTML = html;
  return txt.value;
};

const NewsItem: React.FC<NewsItemProps> = ({ title, description, url, formattedPublishedDate }) => {
  return (
    <div className="news-item">
      <h2>{decodeHtml(title)}</h2>
      <p className="publication-date">Published on: {formattedPublishedDate}</p>
      <p>{decodeHtml(description)}</p>
      <a href={url} target="_blank" rel="noopener noreferrer">Read more</a>
    </div>
  );
};

export default NewsItem;
