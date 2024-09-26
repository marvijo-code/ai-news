import React, { useState } from 'react';
import { Modal, TextField, Button, Box, Typography } from '@mui/material';

interface EmailShareModalProps {
  open: boolean;
  onClose: () => void;
  newsItem: {
    title: string;
    url: string;
  };
}

const EmailShareModal: React.FC<EmailShareModalProps> = ({ open, onClose, newsItem }) => {
  const [email, setEmail] = useState('');

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    // TODO: Implement email sending logic
    console.log(`Sharing article "${newsItem.title}" with ${email}`);
    onClose();
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box sx={{
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)',
        width: 400,
        bgcolor: 'background.paper',
        boxShadow: 24,
        p: 4,
      }}>
        <Typography variant="h6" component="h2" gutterBottom>
          Share Article via Email
        </Typography>
        <form onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="Recipient Email"
            variant="outlined"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            margin="normal"
          />
          <Button type="submit" variant="contained" color="primary" fullWidth sx={{ mt: 2 }}>
            Send
          </Button>
        </form>
      </Box>
    </Modal>
  );
};

export default EmailShareModal;
